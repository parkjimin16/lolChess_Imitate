using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_MoneyLightning : BaseAugmenter
{

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.UserGold += 8;
    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {
        user.UserGold += 1;
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
