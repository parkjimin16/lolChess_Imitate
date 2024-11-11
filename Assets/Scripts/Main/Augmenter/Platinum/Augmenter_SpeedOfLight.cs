using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_SpeedOfLight : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.MapInfo.ItemTile[0].GenerateItem("A002");
        user.MapInfo.ItemTile[0].GenerateItem("B009");
        user.MapInfo.ItemTile[0].GenerateItem("B012");
    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {

    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
