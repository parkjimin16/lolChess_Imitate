using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_BandOfThieves_First : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        Manager.Item.CreateItem("B036", new Vector3(0, 0, 0));
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
