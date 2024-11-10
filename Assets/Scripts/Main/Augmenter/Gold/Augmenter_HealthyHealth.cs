using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_HealthyHealth : BaseAugmenter
{
    // ���� ����

    private ChampionCost cost = ChampionCost.TwoCost;
    private int plusHp;
    private int count;
    #region �ʱ�ȭ

    private void InitValue()
    {
        count = 0;
        plusHp = 80;
    }

    #endregion

    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);
        count = 0;

        foreach (var cBase in list)
        {
            if (cBase.ChampionCost == cost)
            {
                count++;
            }
        }

        foreach (var cBase in list)
        {
            cBase.Augmenter_MaxHP += (plusHp * count);
            cBase.UpdateChampmionStat();

            cBase.ChampionHpMpController.AddHealth(plusHp, 1.0f);
            cBase.UpdateChampmionStat();
        }
    }

    public override void ApplyEndRound(UserData user)
    {
        InitValue();

        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            if (cBase.ChampionCost == cost)
            {
                cBase.InitAugmenterStat();
                cBase.UpdateChampmionStat();
                cBase.ResetHealth();
            }
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
