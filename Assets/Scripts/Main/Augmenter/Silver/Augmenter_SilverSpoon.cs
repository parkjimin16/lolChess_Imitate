using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_SilverSpoon : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        user.UserExp += 10;
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
