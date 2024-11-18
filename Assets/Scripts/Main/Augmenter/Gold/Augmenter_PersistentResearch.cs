using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augmenter_PersistentResearch : BaseAugmenter
{
    private int loseValue;
    private int winValue;

    private int loseTarget;
    private int winTarget;

    #region 증강체 로직
    public override void ApplyNow(UserData user)
    {
        loseValue = 3;
        winValue = 2;

        loseTarget = 0;
        winTarget = 0;
    }

    public override void ApplyStartRound(UserData user)
    {
        winTarget = user.UserSuccessiveWin;
        loseTarget = user.UserSuccessiveLose;

    }

    public override void ApplyEndRound(UserData user)
    {
        if(winTarget < user.UserSuccessiveWin)
        {
            user.UserExp += winValue;
        }
        else if(loseTarget < user.UserSuccessiveLose)
        {
            user.UserExp += loseValue;
        }
    }
    public override void ApplyWhenever(UserData user)
    {

    }
    #endregion
}
