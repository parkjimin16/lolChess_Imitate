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
            // 라운드 실행 X
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

        }
        public override void Execute(ChampionBase champion)
        {
            // 전투 시작
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

        }
        public override void Execute(ChampionBase champion)
        {
            if (champion.ChampionAttackController.IsAttack)
            {
                champion.ChampionAttackController.AttackLogic();
            }
        }
        public override void Exit(ChampionBase champion)
        {

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
