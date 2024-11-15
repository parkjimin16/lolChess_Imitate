using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAndTimeSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 시공간 로직 변수
    private float ap_Power;
    private float hp_Heal;
    private float ad_Speed;
    private float timer;
    private bool isStop;

    public SpaceAndTimeSynergy()
        : base("시공간", ChampionLine.SpaceAndTime, ChampionJob.None, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            ap_Power = 0;
            hp_Heal = 0;
            ad_Speed = 0;
            timer = 0;
            isStop = false;
            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            ap_Power = 35;
            hp_Heal = 0.15f;
            ad_Speed = 0;
            timer = 0;
            isStop = false;
        }
        else if (level >= 4 && level < 6)
        {
            ap_Power = 55;
            hp_Heal = 0.15f;
            ad_Speed = 0;
            timer = 3;
            isStop = true;
        }
        else if (level >= 6)
        {
            ap_Power = 70;
            hp_Heal = 1;
            ad_Speed = 0.35f;
            timer = 4;
            isStop = true;
        }

        //Debug.Log($"[시공간] 레벨 {level} 효과 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.SpaceAndTime && cBase.ChampionLine_Second != ChampionLine.SpaceAndTime)
                continue;


            cBase.Synergy_AP_Power -= ap_Power;
            cBase.Synergy_Atk_Spd -= ad_Speed;

            cBase.UpdateChampmionStat();
        }

        ap_Power = 0;
        hp_Heal = 0;
        ad_Speed = 0;
        timer = 0;
        isStop = false;

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        if(level >= 4)
            SpaceTimeLogic(user, true);
        else
            SpaceTimeLogic(user, false);

    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 시공간 로직

    private void SpaceTimeLogic(UserData user, bool isStopTime)
    {
        var list = GetTeamChampionBase(user);

        foreach(var champion in list)
        {
            int curHp = (int)(champion.Champion_CurHp * hp_Heal);
            champion.ChampionHpMpController.AddHealth(curHp, 1.0f);

            if (champion.ChampionLine_First != ChampionLine.SpaceAndTime && champion.ChampionLine_Second != ChampionLine.SpaceAndTime)
                continue;

            champion.Synergy_AP_Power += ap_Power;
            champion.Synergy_Atk_Spd += ad_Speed;

            champion.UpdateChampmionStat();
        }

        if(timer != 0 && isStop)
            CoroutineHelper.StartCoroutine(ReduceEnemyAttackPowerForDuration(user, timer));
    }

    private List<ChampionBase> GetTeamChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase != null)
                list.Add(cBase);
        }

        return list;
    }


    private List<ChampionBase> GetEnemyChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase != null)
                list.Add(cBase);
        }


        return list;
    }

    private IEnumerator ReduceEnemyAttackPowerForDuration(UserData user, float duration)
    {
        var enemyList = GetEnemyChampionBase(user);

        foreach (var enemy in enemyList)
        {
            if (enemy != null)
            {
                Debug.Log("적 챔피언 스탯");
                enemy.Synergy_AD_Power -= enemy.Champion_AD_Power;
                enemy.Synergy_Speed -= enemy.Champion_Speed;
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (var enemy in enemyList)
        {
            if (enemy != null)
            {
                enemy.Synergy_AD_Power += enemy.Champion_AD_Power;
                enemy.Synergy_Speed += enemy.Champion_Speed;
            }
        }

        yield return null;
    }
    #endregion
}
