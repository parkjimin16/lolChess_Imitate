using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionSynergy : SynergyBase
{
    // 시너지 변수
    private int level;

    // 폭파단 로직 변수
    private float total_Power_First;
    private float total_Power_Second;

    private Coroutine demoCoroutine;

    public DemolitionSynergy()
        : base("폭파단", ChampionLine.None, ChampionJob.Demolition, 0) { }

    #region 활성 & 비활성화

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            total_Power_First = 0;
            total_Power_Second = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            total_Power_First = 0.12f;
            total_Power_Second = 0.3f;
        }
        else if (level >= 4 && level < 6)
        {
            total_Power_First = 0.35f;
            total_Power_Second = 0.65f;
        }
        else if (level >= 6)
        {
            total_Power_First = 0.45f;
            total_Power_Second = 1f;
        }


        Debug.Log($"[폭파단] 레벨 {level} 효과 적용");
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

        if (demoCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(demoCoroutine);
            demoCoroutine = null;
        }

        Debug.Log($"{Name} 시너지가 비활성화되었습니다.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        DemolitionLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region 폭파단 로직
    private void DemolitionLogic(UserData user)
    {
        if (demoCoroutine != null)
            CoroutineHelper.StopCoroutine(demoCoroutine);

        var list = GetDemoChampionBase(user);

        foreach (var demo in list)
        {
            demoCoroutine = CoroutineHelper.StartCoroutine(DemoCoroutine(demo));
        }
    }

    private IEnumerator DemoCoroutine(ChampionBase demo)
    {
        while (true)
        {
            demo.Synergy_Power_Upgrade += total_Power_First;
            demo.UpdateChampmionStat();

            yield return new WaitForSeconds(5f);

            demo.Synergy_Power_Upgrade -= total_Power_First;
            demo.Synergy_Power_Upgrade += total_Power_Second;
            demo.UpdateChampmionStat();
            yield return new WaitForSeconds(3f);

            demo.Synergy_Power_Upgrade -= total_Power_Second;
        }
    }

    private List<ChampionBase> GetDemoChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Demolition && cBase.ChampionJob_Second != ChampionJob.Demolition)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
