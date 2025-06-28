using UnityEngine;

namespace ToB.Entities
{
    public static class EnemyAnimationString
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int VelocityY = Animator.StringToHash("VelocityY");
        public static readonly int Roll = Animator.StringToHash("Roll");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int Die = Animator.StringToHash("Die");

        public static readonly int MotionCancel = Animator.StringToHash("MotionCancel");
    }
}