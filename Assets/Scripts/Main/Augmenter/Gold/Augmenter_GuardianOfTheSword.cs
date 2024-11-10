using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_GuardianOfTheSword : BaseAugmenter
{
    // ���� ����
    private float ad_Power;


    #region �ʱ�ȭ

    private void InitValue()
    {
        ad_Power = 0.15f;
    }

    #endregion

    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();

        user.MapInfo.ItemTile[0].GenerateItem("A001");
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.Augmenter_AD_Power += ad_Power;
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
