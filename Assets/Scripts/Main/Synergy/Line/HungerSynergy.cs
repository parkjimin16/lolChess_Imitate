using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerSynergy : SynergyBase
{
    // 시너지 변수
    private int level;


    // 허기 로직 변수
    private float totalDamage;


    public HungerSynergy()
        : base("허기", ChampionLine.Hunger, ChampionJob.None, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 1)
        {
            totalDamage = 0;

            Deactivate(user);
            return;
        }
        else if (level == 1)
        {
            totalDamage = (100 - user.UserHealth) * 0.008f;
        }

        //Debug.Log($"[허기] 레벨 {level} 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Hunger && cBase.ChampionLine_Second != ChampionLine.Hunger)
                continue;

            cBase.Synergy_Power_Upgrade -= totalDamage;

            cBase.UpdateChampmionStat();
        }


        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 1)
            return;

        SetHungerChampion(user);
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 허기 로직

    private void SetHungerChampion(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Hunger && cBase.ChampionLine_Second != ChampionLine.Hunger)
                continue;

            cBase.Synergy_Power_Upgrade += totalDamage;

            cBase.UpdateChampmionStat();
        }
    }
    #endregion
}
