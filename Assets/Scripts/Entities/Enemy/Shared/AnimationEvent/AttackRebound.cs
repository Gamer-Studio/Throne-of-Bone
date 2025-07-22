using System;
using UnityEngine;

namespace ToB.Entities
{
    public class AttackRebound : MonoBehaviour
    {
        public Enemy owner;
        public float reboundPower;
        
        public float damp;
        public bool isGroundUnit;
        private void Update()
        {
            if (!owner.Physics.externalVelocity.TryGetValue(EnemyPhysicsKeys.REBOUND, out var reboundVector)) return;
            if (reboundVector == Vector2.zero) return;
            
            owner.Physics.externalVelocity[EnemyPhysicsKeys.REBOUND]
                = Vector2.Lerp(reboundVector, Vector2.zero, damp * Time.deltaTime);

            if (Mathf.Abs(reboundVector.x) < 0.005f)
            {
                owner.Physics.externalVelocity[EnemyPhysicsKeys.REBOUND] = Vector2.zero;
                return;
            }
            
            if (isGroundUnit)
            {
                if (owner.Physics.IsLedgeOnSide(reboundVector.x < 0 ? Vector2.left : Vector2.right))
                {
                    owner.Physics.externalVelocity[EnemyPhysicsKeys.REBOUND] = Vector2.zero;
                }
            }
        }

        public void OnRebound()
        {
            owner.Physics.externalVelocity[EnemyPhysicsKeys.REBOUND] =
                reboundPower * owner.LookDirectionHorizontal * -1;
        }
    }
}
