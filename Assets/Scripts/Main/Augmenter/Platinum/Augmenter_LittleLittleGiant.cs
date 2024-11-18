using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_LittleLittleGiant : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {

    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {
        user.UserGold += 2;
        user.SetUserHealth(2);
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
