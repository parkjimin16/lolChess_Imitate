using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ExchangeMarket : BaseAugmenter
{
    #region ����ü ����
    public override void ApplyNow(UserData user)
    {

    }

    public override void ApplyStartRound(UserData user)
    {
        user.UserGold += 3;
    }

    public override void ApplyEndRound(UserData user)
    {

    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
