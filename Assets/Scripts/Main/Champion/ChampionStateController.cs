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
                Debug.Log("Idle State");

                break;

            case ChampionState.Move:
                if (!MergeScene.BatteStart)
                {
                    Debug.Log("여기에요");
                    return;
                }

                if (cBase.ChampionAttackController.IsAttack)
                {
                    ChangeState(ChampionState.Attack, cBase);
                }

                break;

            case ChampionState.Attack:
                if (!MergeScene.BatteStart)
                {
                    return;
                }

                Debug.Log("Attack State");

                if (!cBase.ChampionAttackController.IsAttack)
                {

                }
                break;

            case ChampionState.Die:
                if (!MergeScene.BatteStart)
                {
                    return;
                }

                Debug.Log("Die State");
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
                fsm.ChangeState(new AttackState(cBase), cBase);
                break;
        }

        OnStateChanged?.Invoke(newState); 
    }
}
