using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ToB.Entities
{
    public class FlyIdleState:EnemyPattern
    {
        private readonly Fly owner;
        private readonly FlyFSM fsm;
        Coroutine idleCoroutine;
        public FlyIdleState(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
            owner = enemy as Fly;
            fsm = strategy as FlyFSM;
        }

        public override void Enter()
        {
            base.Enter();

            if (!owner.Hive.IsFlyInPatrolArea(owner))
            {
                fsm.ChangePattern(fsm.returnState);
                return;           
            }
            idleCoroutine = enemy.StartCoroutine(ChangeToWander());
            owner.Physics.externalVelocity[FlyFSM.FLY_KEY] = Vector2.zero;
        }

        IEnumerator ChangeToWander()
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            
            fsm.ChangePattern(fsm.wanderState);
        }

        public override void Exit()
        {
            base.Exit();
            if(idleCoroutine != null) enemy.StopCoroutine(idleCoroutine);
        }
    }
}
