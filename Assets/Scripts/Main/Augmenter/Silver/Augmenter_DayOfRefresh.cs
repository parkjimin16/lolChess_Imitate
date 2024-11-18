using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_DayOfRefresh : BaseAugmenter
{
    private int maxCount;
    private int count;

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        count = 0;
        maxCount = 10;
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

        if (count <= maxCount)
        {
            count++;
            user.UserGold += 2;
        }

        count++;
    }
    #endregion
}
