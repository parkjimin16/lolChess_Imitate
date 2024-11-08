using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class PyroSynergy : SynergyBase
{
    // �ó��� ����
    private int level;


    // ȭ�� ���� ����
    public bool isFirst = true;
    private int fireCount;
    private float atk_Speed;
    private float atk_Power;

    public PyroSynergy()
        : base("ȭ��", ChampionLine.Pyro, ChampionJob.None, 0) { isFirst = true; }

    #region Ȱ�� & ��Ȱ��ȭ

    protected override void ApplyEffects(UserData user, int _level)
    {
        if (isFirst)
        {
            fireCount = 0;
            isFirst = false;
        }

        level = _level;

        if (level < 2)
        {
            atk_Speed = 0;
            atk_Power = 0;
            Deactivate(user);
            return;
        }
        else if (level == 2)
        {
            atk_Speed = 0.05f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level == 3)
        {
            atk_Speed = 0.25f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level == 4)
        {
            atk_Speed = 0.5f + fireCount * 0.02f;
            atk_Power = 0.1f;

        }
        else if (level >= 5)
        {
            atk_Speed = 0.75f + fireCount * 0.02f;
            atk_Power = 0.25f;
        }

        Debug.Log($"[ȭ��] ���� {level} ����");
    }

    protected override void RemoveEffects(UserData user)
    {
        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Pyro && cBase.ChampionLine_Second != ChampionLine.Pyro)
                continue;

            cBase.Synergy_Atk_Spd -= atk_Speed;
            cBase.Synergy_AD_Power -= atk_Power;

            cBase.UpdateChampmionStat();
        }


        Debug.Log($"{Name} �ó����� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public override void Activate(UserData user)
    {
        if (level < 2)
            return;

        SetPyroChampion(user);
    }


    public override void Deactivate(UserData user)
    {
        RemoveEffects(user);
    }

    #endregion

    #region ȭ�� ����
    private void SetPyroChampion(UserData user)
    {
        int count = 0;

        foreach (var champion in user.BattleChampionObject)
        {
            ChampionBase cBase = champion.GetComponent<ChampionBase>();

            if (cBase == null)
                continue;

            if (cBase.ChampionLine_First != ChampionLine.Pyro && cBase.ChampionLine_Second != ChampionLine.Pyro)
                continue;


            count++;
            cBase.Synergy_Atk_Spd += atk_Speed;
            cBase.Synergy_AD_Power += atk_Power;

            cBase.UpdateChampmionStat();
        }

        fireCount += count;
    }
 
    #endregion
}
