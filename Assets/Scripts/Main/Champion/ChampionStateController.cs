using ChampionOwnedStates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionStateController : MonoBehaviour
{
    private ChampionBase cBase;
    private IState currentState;

    private IdleState idleState;
    private MoveState moveState;
    private AttackState attackState;
    private SkillState skillState;
    private DieState dieState;


    public void Init(ChampionBase championBase)
    {
        cBase = championBase;
    }


    public void ChangeState(IState newState)
    {
        if(currentState != null)
        {
            currentState.Exit(cBase);
        }

        currentState = newState;

        if(currentState != null)
        {
            currentState.Enter(cBase);
        }
    }


    private void Awake()
    {
        idleState = new IdleState();
        moveState = new MoveState();
        attackState = new AttackState();
        skillState = new SkillState();
        dieState = new DieState();
    }

    private void Start()
    {
        ChangeState(idleState);
    }

    private void Update()
    {
        if(currentState != null)
        {
            currentState.Execute(cBase);
        }

        AttackLogic();
    }

    private void AttackLogic()
    {
        if (!cBase.ChampionAttackController.IsAttack)
        {
            cBase.ChampionAttackController.AttackLogic();
        }
    }
}
