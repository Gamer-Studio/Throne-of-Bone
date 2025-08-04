using System;
using UnityEngine;

namespace ToB.Entities
{
    public class HitReactionPattern:EnemyPattern
    {
        // 나중에 필요하면 SO에 인터페이스 걸고 가져오는 걸로
        private float hitReactionTime = 0.3f;
        private float enterTime;
        
        public HitReactionPattern(EnemyStrategy strategy, Action EndCallback = null) : base(strategy, EndCallback)
        {
        }

        public override void Enter()
        {
            base.Enter();
            enemy.Animator.SetTrigger(EnemyAnimationString.Hit);
            enterTime = Time.time;
        }

        public override void Execute()
        {
            base.Execute();
            if (Time.time - enterTime > hitReactionTime)
            {
                strategy.ChangeToDefaultPattern();
                enemy.Animator.Rebind();
                enemy.Animator.Update(0f);
            }
        }
    }
}