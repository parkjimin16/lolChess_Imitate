using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonSynergy : SynergyBase
{
    // �ó��� ����
    private int level;


    // �� ���� ����
    private float totalDamage;


    public DragonSynergy()
        : base("��", ChampionLine.Dragon, ChampionJob.None, 0) { }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        level = _level;

        if (level < 2)
        {
            totalDamage = 0;

            Deactivate(user);
            return;
        }
        else if (level == 2)
        {
            totalDamage = 0.1f;
        }
        else if (level == 3)
        {
            totalDamage = 0.3f;
        }

        Debug.Log($"[��] ���� {level} ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach(var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Dragon && cBase.ChampionLine_Second != ChampionLine.Dragon)
                continue;

            cBase.Synergy_Power_Upgrade -= totalDamage;

            cBase.UpdateChampmionStat();
        }


        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        SetDragonChampion(user);
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region �� ����

    private void SetDragonChampion(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null || cBase.ChampionLine_First != ChampionLine.Dragon && cBase.ChampionLine_Second != ChampionLine.Dragon)
                continue;

            cBase.Synergy_Power_Upgrade += totalDamage;

            cBase.UpdateChampmionStat();
        }
    }
    #endregion
}
