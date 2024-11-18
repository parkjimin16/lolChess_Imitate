using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_SilverTicket : BaseAugmenter
{
    private int maxCount;
    private int curCount;

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        maxCount = 4;
        curCount = 0;
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

        if(curCount == maxCount)
        {
            user.UserGold += 2;
            curCount = 0;
        }
        else
        {
            curCount++;
        }

    }
    #endregion
}
