using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ExplosiveGrowth : BaseAugmenter
{
    private int max;
    private int cur;
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        max = 4;
        cur = 0;
    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {
        if(cur < max)
        {
            user.UserExp += 8;
            cur++;
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
