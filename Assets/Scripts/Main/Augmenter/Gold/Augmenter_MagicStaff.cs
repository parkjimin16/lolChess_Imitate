using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_MagicStaff : BaseAugmenter
{
    // ���� ����
    private float ap_Power;

    #region �ʱ�ȭ

    private void InitValue()
    {
        ap_Power = 18f;
    }

    #endregion

    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();

        user.MapInfo.ItemTile[0].GenerateItem("A005");
    }

    public override void ApplyStartRound(UserData user)
    {
        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.Augmenter_AP_Power += ap_Power;
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
