using ChampionOwnedStates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionStateController : MonoBehaviour
{
    private StateManager state;
    private ChampionBase cBase;

    [SerializeField] private ChampionState curState;

    public event Action<ChampionState> OnStateChanged;


    public ChampionState CurState
    {
        get { return curState; }
        set { curState = value; }
    }

    public void Init(ChampionBase championBase)
    {
        cBase = championBase;

        curState = ChampionState.Idle;
        state = new StateManager(new IdleState(cBase));
    }


    private void Update()
    {
        state.UpdateState(cBase);
    }

    public void ChangeState(ChampionState newState, ChampionBase cBase)
    {
        curState = newState;
        
        switch (curState)
        {
            case ChampionState.Idle:
                state.ChangeState(new IdleState(cBase), cBase);
                break;
            case ChampionState.Move:
                state.ChangeState(new MoveState(cBase), cBase);
                break;
            case ChampionState.Attack:
                state.ChangeState(new AttackState(cBase), cBase);
                break;
            case ChampionState.Die:
                state.ChangeState(new DieState(cBase), cBase);
                break;
        }
        
        OnStateChanged?.Invoke(newState); 
    }
}
