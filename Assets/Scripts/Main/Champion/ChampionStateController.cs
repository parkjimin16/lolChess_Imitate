using ChampionOwnedStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionStateController : MonoBehaviour
{
    private FSMManager fsm;
    private ChampionBase cBase;

    [SerializeField] private ChampionState curState;

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
                if (TestScene.GameStart)
                {
                    ChangeState(ChampionState.Move, cBase);
                }
                break;

            case ChampionState.Move:
                if (cBase.ChampionAttackController.IsAttack)
                {
                    ChangeState(ChampionState.Attack, cBase);
                }

                break;

            case ChampionState.Attack:
                if (!cBase.ChampionAttackController.IsAttack)
                {
                    ChangeState(ChampionState.Move, cBase);
                }


                if(!TestScene.GameStart) 
                {
                    ChangeState(ChampionState.Idle, cBase);
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
    }
}
