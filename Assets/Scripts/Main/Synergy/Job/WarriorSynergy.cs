using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarriorSynergy : SynergyBase
{
    // �ó��� ����
    private int level;

    // ���� ���� ����
    private float blood_Suck;
    private float total_Damage;
    private float total_Def;

    private Coroutine atkCoroutine;

    private System.Random random = new System.Random();

    public WarriorSynergy()
        : base("����", ChampionLine.None, ChampionJob.Warrior, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            blood_Suck = 0;
            total_Damage = 0;
            total_Def = 0;

            Deactivate(user);
        }
        else if (level >= 2 && level < 4)
        {
            blood_Suck = 0.1f;
            total_Damage = 0.1f;
            total_Def = 0;
        }
        else if (level >= 4 && level < 6)
        {
            blood_Suck = 0.2f;
            total_Damage = 0.2f;
            total_Def = 0;
        }
        else if (level >= 6)
        {
            blood_Suck = 0.25f;
            total_Damage = 0.25f;
            total_Def = 0.15f;
        }


        //Debug.Log($"[����] ���� {level} ȿ�� ����");
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

        //Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        WarriorLogic(user);
    }

    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ���� ����

    private void WarriorLogic(UserData user)
    {
        if (atkCoroutine != null)
            CoroutineHelper.StopCoroutine(atkCoroutine);

        var list = GetWarriorChampionBase(user);

        foreach (var warrior in list)
        {
            atkCoroutine = CoroutineHelper.StartCoroutine(WarriorCoroutine(warrior));
        }
    }
    private IEnumerator WarriorCoroutine(ChampionBase warrior)
    {
        warrior.Synergy_Power_Upgrade += total_Damage;
        warrior.Synergy_Blood_Suck += blood_Suck;
        warrior.Synergy_Total_Def += total_Def;

        while (true)
        {
            if (warrior.Champion_CurHp <= warrior.Champion_MaxHp * 0.7f)
            {
                warrior.Synergy_Power_Upgrade += total_Damage;
                yield break;
            }

            yield return null;
        }
    }
    private List<ChampionBase> GetWarriorChampionBase(UserData user)
    {
        var list = new List<ChampionBase>();

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionJob_First != ChampionJob.Warrior && cBase.ChampionJob_Second != ChampionJob.Warrior)
                continue;

            list.Add(cBase);
        }

        return list;
    }
    #endregion
}
