using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ���Ĵ� ���� ����
    private float total_Power_First;
    private float total_Power_Second;

    private Coroutine demoCoroutine;

    public DemolitionSynergy()
        : base("���Ĵ�", ChampionLine.None, ChampionJob.Demolition, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

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


        Debug.Log($"[���Ĵ�] ���� {level} ȿ�� ����");
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

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
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

    #region ���Ĵ� ����
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
