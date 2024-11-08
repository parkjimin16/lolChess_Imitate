using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class PyroSynergy : SynergyBase
{
    // 시너지 변수
    private int level;


    // 화염 로직 변수
    public bool isFirst = true;
    private int fireCount;
    private float atk_Speed;
    private float atk_Power;

    public PyroSynergy()
        : base("화염", ChampionLine.Pyro, ChampionJob.None, 0) { isFirst = true; }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        if (isFirst)
        {
            fireCount = 0;
            isFirst = false;
        }

        level = _level;

        if (level < 2)
        {
            atk_Speed = 0;
            atk_Power = 0;
            Deactivate(user);
            return;
        }
        else if (level == 2)
        {
            atk_Speed = 0.05f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level == 3)
        {
            atk_Speed = 0.25f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level == 4)
        {
            atk_Speed = 0.5f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level >= 5)
        {
            atk_Speed = 0.75f + fireCount * 0.02f;
            atk_Power = 0.25f;
        }

        Debug.Log($"[화염] 레벨 {level} 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Pyro && cBase.ChampionLine_Second != ChampionLine.Pyro)
                continue;

            cBase.Synergy_Atk_Spd -= atk_Speed;
            cBase.Synergy_AD_Power -= atk_Power;

            cBase.UpdateChampmionStat();
        }


        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        SetPyroChampion(user);
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 화염 로직
    private void SetPyroChampion(UserData user)
    {
        int count = 0;

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Pyro && cBase.ChampionLine_Second != ChampionLine.Pyro)
                continue;


            count++;
            cBase.Synergy_Atk_Spd += atk_Speed;
            cBase.Synergy_AD_Power += atk_Power;

            cBase.UpdateChampmionStat();
        }

        fireCount += count;
    }
 
    #endregion
}
