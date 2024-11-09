using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScholarSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ���� ���� ����
    private int mana;
    private float ap_Power;

    private Coroutine scholarCoroutine;

    public ScholarSynergy()
        : base("����", ChampionLine.None, ChampionJob.Scholar, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            mana = 0;
            ap_Power = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            mana = 3;
            ap_Power = 10;
        }
        else if (level >= 4 && level < 6)
        {
            mana = 6;
            ap_Power = 20;
        }
        else if (level >= 6)
        {
            mana = 12;
            ap_Power = 30;
        }


        Debug.Log($"[����] ���� {level} ȿ�� ����");
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

        if (scholarCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(scholarCoroutine);
            scholarCoroutine = null; 
        }

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        ScholarLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ���� ����

    private void ScholarLogic(UserData user)
    {
        if (scholarCoroutine != null)
            CoroutineHelper.StopCoroutine(scholarCoroutine);

        var list = GetScholarChampionBase(user);

        foreach (var demo in list)
        {
            demo.Synergy_AP_Power += ap_Power;
            demo.UpdateChampmionStat();
            scholarCoroutine = CoroutineHelper.StartCoroutine(ScholarCoroutine(demo));
        }
    }

    private IEnumerator ScholarCoroutine(ChampionBase demo)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            demo.ChampionHpMpController.ManaPlus(mana);
        }
    }

    private List<ChampionBase> GetScholarChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Scholar && cBase.ChampionJob_Second != ChampionJob.Scholar)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
