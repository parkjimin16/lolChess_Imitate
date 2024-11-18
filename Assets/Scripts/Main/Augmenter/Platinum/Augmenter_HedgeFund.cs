using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_HedgeFund : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.UserGold += 22;
    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {
        if(user.UserGold >= 60)
        {
            int plus = user.UserGold - 50;

            plus /= 10;

            if(plus >= 5)
            {
                plus = 5;
            }

            user.UserGold += plus;
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
