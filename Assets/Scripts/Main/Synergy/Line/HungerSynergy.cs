using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerSynergy : SynergyBase
{
    // �ó��� ����
    private int level;


    // ��� ���� ����
    private float totalDamage;


    public HungerSynergy()
        : base("���", ChampionLine.Hunger, ChampionJob.None, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

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

        //Debug.Log($"[���] ���� {level} ����");
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


        //Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
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

    #region ��� ����

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
