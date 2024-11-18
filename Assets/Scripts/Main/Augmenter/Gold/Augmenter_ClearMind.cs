using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_ClearMind : BaseAugmenter
{
    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {

    }

    public override void ApplyStartRound(UserData user)
    {

    }

    public override void ApplyEndRound(UserData user)
    {
        if(user.NonBattleChampionObject.Count == 0)
        {
            user.UserExp += 3;
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
