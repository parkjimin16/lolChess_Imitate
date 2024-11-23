using ChampionOwnedStates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionStateController : MonoBehaviour
{
    private FSMManager fsm;
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
        fsm = new FSMManager(new IdleState(cBase));
    }


    private void Update()
    {
        switch (curState)
        {
            case ChampionState.Idle:
                break;

            case ChampionState.Move:
                if (!MergeScene.BatteStart)
                {
                    return;
                }


                break;

            case ChampionState.Attack:
                if (!MergeScene.BatteStart)
                {
                    return;
                }

                break;

            case ChampionState.Die:
                if (!MergeScene.BatteStart)
                {
                    return;
                }


                break;
        }

        fsm.UpdateState(cBase);
    }

    public void ChangeState(ChampionState newState, ChampionBase cBase)
    {
        curState = newState;

        switch(curState)
        {
            case ChampionState.Idle:
                fsm.ChangeState(new IdleState(cBase), cBase);
                break;
            case ChampionState.Move:
                fsm.ChangeState(new MoveState(cBase), cBase);
                break;
            case ChampionState.Attack:
                fsm.ChangeState(new AttackState(cBase), cBase);
                break;
            case ChampionState.Die:
                fsm.ChangeState(new DieState(cBase), cBase);
                break;
        }

        OnStateChanged?.Invoke(newState); 
    }
}
