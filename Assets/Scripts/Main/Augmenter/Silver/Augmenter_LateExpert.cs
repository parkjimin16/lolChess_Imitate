using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_LateExpert : BaseAugmenter
{
    private int targetLevel;
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        targetLevel = 9;
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

    public override void ApplyLevelUp(UserData user)
    {
        base.ApplyLevelUp(user);

        if(targetLevel == user.UserLevel)
        {
            user.UserGold += 33;
        } 
    }
    #endregion
}
