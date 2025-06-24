using UnityEngine;

namespace ToB
{
    public static class EnemyAnimationString
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int VelocityY = Animator.StringToHash("VelocityY");
    }
}