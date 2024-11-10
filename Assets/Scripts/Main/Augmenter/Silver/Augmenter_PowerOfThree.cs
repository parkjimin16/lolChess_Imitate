using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_PowerOfThree : BaseAugmenter
{
    // ���� ����

    private ChampionCost cost = ChampionCost.ThreeCost;

    private int plusHp;
    private int plusMana;
    private float atk_Speed;

    #region �ʱ�ȭ

    private void InitValue()
    {
        plusHp = 150;
        plusMana = 10;
        atk_Speed = 0.1f;
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

        foreach (var cBase in list)
        {
            if (cBase.ChampionCost == cost)
            {
                Debug.Log("ü�� : " + plusHp);
                cBase.Augmenter_MaxHP += plusHp;
                cBase.UpdateChampmionStat();
                
                cBase.ChampionHpMpController.AddHealth(plusHp, 1.0f);
                cBase.ChampionHpMpController.ManaPlus(plusMana);
                cBase.Augmenter_Atk_Spd += atk_Speed;

                cBase.UpdateChampmionStat();
            }
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
    #endregion
}
