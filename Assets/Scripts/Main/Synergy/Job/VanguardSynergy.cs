using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VanguardSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 선봉대 로직 변수
    private float total_Def_First;
    private float total_Def_Second;

    private Coroutine shieldCoroutine;

    public VanguardSynergy()
        : base("선봉대", ChampionLine.None, ChampionJob.Vanguard, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            total_Def_First = 0;
            total_Def_Second = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            total_Def_First = 0.1f;
            total_Def_Second = 0.18f;
        }
        else if (level >= 4 && level < 6)
        {
            total_Def_First = 0.1f;
            total_Def_Second = 0.35f;
        }
        else if (level >= 6)
        {
            total_Def_First = 0.1f;
            total_Def_Second = 0.7f;
        }


        //Debug.Log($"[선봉대] 레벨 {level} 효과 적용");
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


        if (shieldCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }


        //Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        VanguardLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 선봉대 로직

    private void VanguardLogic(UserData user)
    {
        if (shieldCoroutine != null)
            CoroutineHelper.StopCoroutine(shieldCoroutine);

        var list = GetVanguardChampionBase(user);

        foreach (var vanguard in list)
        {
            shieldCoroutine = CoroutineHelper.StartCoroutine(VanguardShieldCoroutine(vanguard));
        }
    }

    private IEnumerator VanguardShieldCoroutine(ChampionBase vanguard)
    {
        vanguard.Synergy_Total_Def += total_Def_First;

        yield return new WaitForSeconds(5f);

        vanguard.Synergy_Total_Def -= total_Def_First;
        vanguard.Synergy_Total_Def += total_Def_Second;

        while (true)
        {
            if (vanguard.Champion_CurHp <= vanguard.Champion_MaxHp * 0.5f)
            {
                vanguard.Synergy_Total_Def += total_Def_Second;
                yield return new WaitForSeconds(10f);

                vanguard.Synergy_Total_Def -= total_Def_Second;
                yield break;
            }

            yield return null;
        }
    }

    private List<ChampionBase> GetVanguardChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Vanguard && cBase.ChampionJob_Second != ChampionJob.Vanguard)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
