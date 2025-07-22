using UnityEngine;

namespace ToB.Entities
{
    public static class EnemyAnimationString
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Move = Animator.StringToHash("Move");
        public static readonly int Chase = Animator.StringToHash("Chase");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int VelocityY = Animator.StringToHash("VelocityY");
        public static readonly int Roll = Animator.StringToHash("Roll");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int SpecialAttack = Animator.StringToHash("SpecialAttack");
        public static readonly int Die = Animator.StringToHash("Die");
        public static readonly int Sleep = Animator.StringToHash("Sleep");
        public static readonly int WakeUp = Animator.StringToHash("WakeUp");
        public static readonly int Bark = Animator.StringToHash("Bark");
        public static readonly int Hit = Animator.StringToHash("Hit");
        
        public static readonly int MotionCancel = Animator.StringToHash("MotionCancel");
        public static readonly int AttackEnd  = Animator.StringToHash("AttackEnd");
        public static readonly int AttackMiddleEnd  = Animator.StringToHash("AttackMiddleEnd");
    }
}