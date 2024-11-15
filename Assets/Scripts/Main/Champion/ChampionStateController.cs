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
                    Debug.Log("배틀 종료");
                    return;
                }

                Debug.Log("Move State");


                if (cBase.ChampionAttackController.IsAttack)
                {
                    Debug.Log("Move To Attack");
                    ChangeState(ChampionState.Attack, cBase);
                }

                break;

            case ChampionState.Attack:
                if (!MergeScene.BatteStart)
                {
                    Debug.Log("배틀 종료");
                    return;
                }

                Debug.Log("Attack State");

                if (!cBase.ChampionAttackController.IsAttack)
                {
                    //Debug.Log("Attack To Move");
                    //ChangeState(ChampionState.Move, cBase);
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
        }

        OnStateChanged?.Invoke(newState); 
    }
}
