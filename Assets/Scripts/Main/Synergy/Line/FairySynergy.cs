using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairySynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 요정 로직 변수
    private int hp_Upgrade;

    public FairySynergy()
        : base("요정", ChampionLine.Fairy, ChampionJob.None, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 3)
        {
            hp_Upgrade = 0;
            Deactivate(user);
        }
        else if (level >= 3 && level < 5)
        {
            hp_Upgrade = 150;
        }
        else if (level >= 5 && level < 7)
        {
            hp_Upgrade = 400;
        }
        else if (level >= 7 && level < 9)
        {
            hp_Upgrade = 700;
        }
        else if (level >= 9)
        {
            hp_Upgrade = 999;
        }

        //Debug.Log($"[요정] 레벨 {level} 효과 적용");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Fairy && cBase.ChampionLine_Second != ChampionLine.Fairy)
                continue;


            cBase.Synergy_MaxHP -= hp_Upgrade;

            cBase.UpdateChampmionStat();
        }

        hp_Upgrade = 0;

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        FairyLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 요정 로직

    private void FairyLogic(UserData user)
    {
        var list = GetTeamChampionBase(user);

        if (list.Count <= 0)
            return;

        foreach(var cBase in list)
        {
            if (cBase.ChampionLine_First != ChampionLine.Fairy && cBase.ChampionLine_Second != ChampionLine.Fairy)
                continue;

            cBase.Synergy_MaxHP += hp_Upgrade;

            cBase.UpdateChampmionStat();
        }
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

    #endregion
}
