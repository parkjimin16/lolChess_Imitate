using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ������ ���� ����
    private int maxCount;
    private int curCount;

    private int ap_Power_Ratio;
    private float ap_Power_Team;
    private float ap_Power;

    private Coroutine mageCoroutine;

    public MageSynergy()
        : base("������", ChampionLine.None, ChampionJob.Mage, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            maxCount = 40;
            curCount = 0;
            ap_Power_Ratio = 0;
            ap_Power_Team = 0;
            ap_Power = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            maxCount = 40;
            curCount = 0;
            ap_Power_Ratio = 1;
            ap_Power_Team = 10;
            ap_Power = 0;
        }
        else if (level >= 4)
        {
            maxCount = 40;
            curCount = 0;
            ap_Power_Ratio = 2;
            ap_Power_Team = 30;
            ap_Power = 0;
        }

        //Debug.Log($"[������] ���� {level} ȿ�� ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Mage && cBase.ChampionJob_Second != ChampionJob.Mage)
            {

                cBase.Synergy_AP_Power -= ap_Power_Team;
                continue;
            }


            cBase.InitSynergyStat();
            cBase.UpdateChampmionStat();
        }

        if (mageCoroutine != null)
        {
            CoroutineHelper.StopCoroutine(mageCoroutine);
            mageCoroutine = null;
        }

        //Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        MageLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ������ ����

    private void MageLogic(UserData user)
    {
        if (mageCoroutine != null)
            CoroutineHelper.StopCoroutine(mageCoroutine);

        ChampionLogic(user);

        mageCoroutine = CoroutineHelper.StartCoroutine(MageLogicCoroutine(user));

    }

    private void ChampionLogic(UserData user)
    {
        var list = GetTeamChampionBase(user);

        if (list.Count <= 0)
            return;

        foreach (var cBase in list)
        {
            if (cBase.ChampionJob_First == ChampionJob.Mage || cBase.ChampionJob_Second == ChampionJob.Mage)
                continue;

            cBase.Synergy_AP_Power += ap_Power_Team;
            cBase.UpdateChampmionStat();
        }
    }

    private IEnumerator MageLogicCoroutine(UserData user)
    {
        var list = GetTeamChampionBase(user);

        if (list.Count <= 0)
            yield break;

        while (curCount < maxCount)
        {
            foreach (var cBase in list)
            {
                if (cBase.ChampionJob_First != ChampionJob.Mage && cBase.ChampionJob_Second != ChampionJob.Mage)
                    continue;


                ap_Power = curCount * ap_Power_Ratio;
                cBase.Synergy_AP_Power -= ap_Power;

                curCount++;
                ap_Power = curCount * ap_Power_Ratio;
                cBase.Synergy_AP_Power += ap_Power;

                cBase.UpdateChampmionStat();
            }

            yield return new WaitForSeconds(0.7f);
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
