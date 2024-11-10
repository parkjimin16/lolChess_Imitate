using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_WarmUp_First : BaseAugmenter
{
    // ���� ����
    private float atk_Speed;

    #region �ʱ�ȭ

    private void InitValue()
    {
        atk_Speed = 0.08f;
    }

    #endregion

    #region ����ü ����
    public override void ApplyNow(UserData user)
    {
        InitValue();
    }

    public override void ApplyStartRound(UserData user)
    {
        atk_Speed += 0.005f;

        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {

            cBase.Augmenter_Atk_Spd += atk_Speed;

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
