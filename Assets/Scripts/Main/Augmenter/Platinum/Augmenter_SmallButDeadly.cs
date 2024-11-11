using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_SmallButDeadly : BaseAugmenter
{
    // ���� ����
    private float spd;
    private float atk_Spd;


    #region �ʱ�ȭ
    private void InitValue()
    {
        spd = 0.3f;
        atk_Spd = 0.3f;
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
            cBase.Augmenter_Speed += spd;
            cBase.Augmenter_Atk_Spd += atk_Spd;

            cBase.UpdateChampmionStat();

        }
    }

    public override void ApplyEndRound(UserData user)
    {
        InitValue();

        var list = GetUserChampions(user);

        foreach (var cBase in list)
        {
            cBase.InitAugmenterStat();
            cBase.UpdateChampmionStat();
            cBase.ResetHealth();
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
