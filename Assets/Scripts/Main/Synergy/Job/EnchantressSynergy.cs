using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantressSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 요술사 로직 변수
    private float ap_Power_Ratio;

    private System.Random random = new System.Random();

    public EnchantressSynergy()
        : base("요술사", ChampionLine.None, ChampionJob.Enchantress, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 3)
        {
            ap_Power_Ratio = 0;

            Deactivate(user);
        }
        else if (level >= 3 && level < 5)
        {
            ap_Power_Ratio = 1f;
        }
        else if (level >= 5 && level < 7)
        {
            ap_Power_Ratio = 1.3f;
        }
        else if (level >= 7 && level < 10)
        {
            ap_Power_Ratio = 1.7f;
        }
        else if (level >= 10)
        {
            ap_Power_Ratio = 2.8f;
        }


        //Debug.Log($"[요술사] 레벨 {level} 효과 적용");
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

        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 3)
            return;

        EnchantressLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion


    #region 요술사 로직
    private void EnchantressLogic(UserData user)
    {
        var list = GetEnchantressChampionBase(user);

        foreach(var champion in list)
        {
            float value = champion.Champion_AP_Power * ap_Power_Ratio;
            champion.Synergy_AP_Power += value;

            champion.UpdateChampmionStat();
        }
    }

    private List<ChampionBase> GetEnchantressChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Enchantress && cBase.ChampionJob_Second != ChampionJob.Enchantress)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion

}
