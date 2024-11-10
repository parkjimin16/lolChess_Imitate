using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_Impenetrable : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.MapInfo.ItemTile[0].GenerateItem("A007");
        user.MapInfo.ItemTile[0].GenerateItem("B016");
        user.MapInfo.ItemTile[0].GenerateItem("B022");
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
