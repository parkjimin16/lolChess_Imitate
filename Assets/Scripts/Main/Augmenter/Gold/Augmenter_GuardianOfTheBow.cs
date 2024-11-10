using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_GuardianOfTheBow : BaseAugmenter
{
    // 로직 변수
    private float atk_Spd;


    #region 초기화

    private void InitValue()
    {
        atk_Spd = 0.12f;
    }

    #endregion

    #region 증강체 로직
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
