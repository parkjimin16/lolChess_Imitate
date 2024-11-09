using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ��ɲ� ���� ����
    private float ad_Power_First;
    private float ad_Power_Second;
    private float atk_Spd;

    private Coroutine atkCoroutine;

    public HunterSynergy()
        : base("��ɲ�", ChampionLine.None, ChampionJob.Hunter, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            ad_Power_First = 0;
            ad_Power_Second = 0;
            atk_Spd = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            ad_Power_First = 0.15f;
            ad_Power_Second = 0.35f;
            atk_Spd = 0;
        }
        else if (level >= 4 && level < 6)
        {
            ad_Power_First = 0.4f;
            ad_Power_Second = 0.65f;
            atk_Spd = 0;
        }
        else if (level >= 6)
        {
            ad_Power_First = 0.7f;
            ad_Power_Second = 1;
            atk_Spd = 0.2f;
        }


        Debug.Log($"[��ɲ�] ���� {level} ȿ�� ����");
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

        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        ShelterLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ��ɲ� ����

    private void ShelterLogic(UserData user) 
    {
        if (atkCoroutine != null)
            CoroutineHelper.StopCoroutine(atkCoroutine);

        var list = GetHunterChampionBase(user);

        atkCoroutine = CoroutineHelper.StartCoroutine(AttackCoroutine(list));
    }
    private IEnumerator AttackCoroutine(List<ChampionBase> list)
    {
        foreach (var cBase in list)
        {
            cBase.Synergy_AD_Power += ad_Power_First;
            cBase.UpdateChampmionStat();
        }

        yield return new WaitForSeconds(6f);

        foreach (var cBase in list)
        {
            cBase.Synergy_AD_Power -= ad_Power_First;

            cBase.Synergy_AD_Power += ad_Power_Second;
            cBase.Synergy_Atk_Spd += atk_Spd;
            cBase.UpdateChampmionStat();
        }
    }

    private List<ChampionBase> GetHunterChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Hunter && cBase.ChampionJob_Second != ChampionJob.Hunter)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
