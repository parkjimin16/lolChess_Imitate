using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_LittleGiant : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.UserHealthMax += 30;
        user.SetUserHealth(30);

        Manager.UserHp.UpdateHealthBars();
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
    #endregion
}
