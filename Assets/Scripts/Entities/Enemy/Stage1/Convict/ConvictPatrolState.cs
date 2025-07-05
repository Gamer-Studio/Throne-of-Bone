using System;
using UnityEngine;

namespace ToB.Entities
{
    public class ConvictPatrolState:GroundDefaultMovePattern
    {
        private readonly Convict owner;
        ConvictFSM ownerFSM;

        public ConvictPatrolState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            ownerFSM = strategy as ConvictFSM;
           owner = ownerFSM.Owner;
        }

        public override void Enter()
        {
            owner.Animator.SetBool(EnemyAnimationString.Move, true);
        }

        public override void Execute()
        {
            base.Execute();
            if (owner.target)
                owner.FSM.ChangePattern(owner.FSM.chaseState);
        }

        public override void Exit()
        {
            base.Exit();
            owner.Animator.SetBool(EnemyAnimationString.Move, false);
        }
    }
}