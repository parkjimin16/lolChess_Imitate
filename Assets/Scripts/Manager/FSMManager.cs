using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager
{
    private BaseState curState;

    public FSMManager(BaseState state)
    {
        curState = state;
    }

    public void ChangeState(BaseState newState, ChampionBase cBase)
    {
        if (newState == curState)
            return;

        if (curState != null)
            curState.Exit(cBase);

        curState = newState;
        curState.Enter(cBase);
    }

    public void UpdateState(ChampionBase cBase)
    {
        if (curState != null)
            curState.Execute(cBase);
    }
}
