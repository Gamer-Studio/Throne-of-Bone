
using System;
using UnityEngine;

namespace ToB.Entities
{
    public class FlyChaseState:EnemyPattern
    {
        private readonly Fly owner;
        private readonly FlyFSM fsm;
        
        private float Speed => owner.DataSO.FastMovementSpeed;
        public FlyChaseState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Fly;
            fsm = strategy as FlyFSM;
        }

        public override void Enter()
        {
            base.Enter();
            owner.audioPlayer.Play("Insect_Fly_Loop_03",true);
        }

        public override void Execute()
        {
            base.Execute();
            Transform target;
            if (owner.target)
            {
                if (!owner.Hive.target)
                {
                    fsm.ChangePattern(fsm.returnState);
                    return;
                } 
                target = owner.target.transform;
            }
            else
            {
                if(owner.Hive.target)
                    target = owner.Hive.target.transform;
                else
                {
                    fsm.ChangePattern(fsm.returnState);
                    return;
                }
            }

            if (owner.target && owner.TargetInAttackRange)
            {
                fsm.ChangePattern(fsm.attackState);
            }
            else
            {
                Vector2 targetPos = target.position;
                Vector2 direction = (targetPos - (Vector2)owner.transform.position).normalized;
                owner.Physics.externalVelocity[FlyFSM.FLY_KEY] = direction * Speed;
                enemy.LookHorizontal(direction.x > 0 ? Vector2.right : Vector2.left);
            }
        }

        public override void Exit()
        {
            base.Exit();
            owner.audioPlayer.Stop("Insect_Fly_Loop_03");
        }
    }
}
