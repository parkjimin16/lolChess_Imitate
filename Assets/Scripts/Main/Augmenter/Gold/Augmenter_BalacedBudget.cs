using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_BalacedBudget : BaseAugmenter
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
        if(curCount < 4)
        {
            user.UserGold += 7;

            curCount++;
        }
    }

    public override void ApplyEndRound(UserData user)
    {

    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
