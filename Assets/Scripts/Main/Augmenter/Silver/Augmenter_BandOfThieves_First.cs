using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_BandOfThieves_First : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.MapInfo.ItemTile[0].GenerateItem("B036");
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
