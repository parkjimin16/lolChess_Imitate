using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_GuardianOfTheBow : BaseAugmenter
{
    // ���� ����
    private float atk_Spd;


    #region �ʱ�ȭ

    private void InitValue()
    {
        atk_Spd = 0.12f;
    }

    #endregion

    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();

        user.MapInfo.ItemTile[0].GenerateItem("A002");
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.Augmenter_Atk_Spd += atk_Spd;
            cBase.UpdateChampmionStat();
        }
    }

    public override void ApplyEndRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.InitAugmenterStat();
            cBase.UpdateChampmionStat();
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
