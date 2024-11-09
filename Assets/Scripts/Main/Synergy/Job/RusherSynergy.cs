using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RusherSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 쇄도자 로직 변수
    private float percent;
    private float health_Heal;

    private Coroutine atkCoroutine;

    private System.Random random = new System.Random();

    public RusherSynergy()
        : base("쇄도자", ChampionLine.None, ChampionJob.Rusher, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 3)
        {
            percent = 0;
            health_Heal = 0;

            Deactivate(user);
        }
        else if (level >= 3 && level < 5)
        {
            percent = 0.25f;
            health_Heal = 0;
        }
        else if (level >= 5 && level < 7)
        {
            percent = 0.65f;
            health_Heal = 0;
        }
        else if (level >= 7 && level < 9)
        {
            percent = 0.8f;
            health_Heal = 0.03f;
        }
        else if (level >= 9)
        {
            percent = 1;
            health_Heal = 0.1f;
        }


        Debug.Log($"[쇄도자] 레벨 {level} 효과 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            cBase.InitSynergyStat();
            cBase.UpdateChampmionStat();
        }

        if (atkCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(atkCoroutine);
            atkCoroutine = null;
        }

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        RusherLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 쇄도자 로직

    private void RusherLogic(UserData user)
    {
        if (atkCoroutine != null)
            CoroutineHelper.StopCoroutine(atkCoroutine);

        var list = GetRusherChampionBase(user);

        foreach(var cBase in list)
        {
            atkCoroutine = CoroutineHelper.StartCoroutine(AttackCoroutine(cBase));
        }
    }
    private IEnumerator AttackCoroutine(ChampionBase rusher)
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if(random.NextDouble() < percent)
            {
                float originalAttackPower = rusher.Champion_AD_Power;
                float healthAmount = rusher.Champion_MaxHp * health_Heal;
                
                rusher.Synergy_AD_Power = originalAttackPower * 0.33f;

                if(healthAmount > 0)
                    rusher.ChampionHpMpController.AddHealth((int)health_Heal, 1.0f);

                rusher.UpdateChampmionStat();

                yield return new WaitForSeconds(2f);

                rusher.Synergy_AD_Power = 0;
                rusher.UpdateChampmionStat();
            }
        }

    }

    private List<ChampionBase> GetRusherChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Rusher && cBase.ChampionJob_Second != ChampionJob.Rusher)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
