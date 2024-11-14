using System;
using UnityEngine;

namespace ChampionOwnedStates
{
    public class IdleState : BaseState
    {
        public IdleState(ChampionBase championBase) : base(championBase) { }


        public override void Enter(ChampionBase champion)
        {
 
        }
        public override void Execute(ChampionBase champion)
        {
            // ���� ���� X
        }
        public override void Exit(ChampionBase champion)
        {

        }
    }
    public class MoveState : BaseState
    {
        public MoveState(ChampionBase championBase) : base(championBase) { }

        public override void Enter(ChampionBase champion)
        {
            // ���� ����
            champion.ChampionAttackController.FindPathToTarget();
        }
        public override void Execute(ChampionBase champion)
        {

        }
        public override void Exit(ChampionBase champion)
        {

        }
    }
    public class AttackState : BaseState
    {
        public AttackState(ChampionBase championBase) : base(championBase) { }

        public override void Enter(ChampionBase champion)
        {
            champion.ChampionAttackController.AttackLogic();
        }
        public override void Execute(ChampionBase champion)
        {

        }
        public override void Exit(ChampionBase champion)
        {
            champion.ChampionAttackController.AttackLogicStop();
        }
    }


    public class DieState : BaseState
    {
        public DieState(ChampionBase championBase) : base(championBase) { }
        public override void Enter(ChampionBase champion)
        {

        }
        public override void Execute(ChampionBase champion)
        {

        }
        public override void Exit(ChampionBase champion)
        {

        }
    }
}
