using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_PrismTicket : BaseAugmenter
{
    private float percent;

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        percent = 0.45f;
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

    public override void ApplyReroll(UserData user)
    {
        base.ApplyReroll(user);

        if (Random.Range(0, 100) < 45)
        {
            user.UserGold += 2;
        }
    }
    #endregion
}
