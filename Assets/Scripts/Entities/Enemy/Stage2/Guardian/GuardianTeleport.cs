using System;
using UnityEngine;

namespace ToB.Entities
{
    public class GuardianTeleport : MonoBehaviour
    {
        public Guardian guardian;
        public LayerMask groundLayer;
        
        Vector2 nextPosition;

        public float offensiveTeleportDistance;
        private bool avoid;

        enum EDirection
        {
            Front,
            Back
        }
        private void Reset()
        {
            groundLayer = LayerMask.GetMask("Ground");
        }

        public void Teleport()
        {
            if (!guardian.target) return;
            
            Debug.Log("ㅇㅁㅇ");
            if (!avoid)
            {
                TeleportToAttackPosition();
            }
            else
            {
                // 가디언 룸에 TP 포인트 몇군데 잡아서 랜덤픽 해도 괜찮을듯함
                guardian.transform.position = nextPosition;
            }
        }

        private void TeleportToAttackPosition()
        {
            bool canMoveToPoint = MoveToPlayersPoint(EDirection.Back);

            if (!canMoveToPoint)
                canMoveToPoint = MoveToPlayersPoint(EDirection.Front);
            
            // 모두 실패시 제자리 텔레포트
        }

        private bool MoveToPlayersPoint(EDirection eDirection)
        {
            Transform player = guardian.target.transform;
            
            // 플레이어의 GroundChecker 좌표를 직접 보고 가져왔습니다. 일부러 지면에서 살짝 띄울수 있게 했습니다
            Vector2 playerFloorContactPoint = (Vector2)player.position + new Vector2(0, -1.212f + 0.1f) * player.localScale.y;

            Vector2 targetDirection;
            if (eDirection == EDirection.Front)
            {
                targetDirection = player.eulerAngles.y == 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                targetDirection = player.eulerAngles.y == 0 ? Vector2.left : Vector2.right;
            }

            Vector2 movePoint = playerFloorContactPoint + targetDirection * offensiveTeleportDistance;
            
            // 이동 예정 지점에 이동 가능한지 체크
            Vector2 boxOrigin = movePoint + new Vector2(0, guardian.Physics.TerrainSensor.bounds.extents.y);
            RaycastHit2D hit = Physics2D.BoxCast(boxOrigin, guardian.Physics.TerrainSensor.bounds.size, 0, Vector2.up, 0.02f, groundLayer);
            
#if UNITY_EDITOR
// Scene 뷰에서 boxOrigin 위치에 작은 박스 그리기
            Vector3 boxCenter = boxOrigin;
            Vector3 boxSize = guardian.Physics.TerrainSensor.bounds.size;
            
            Color c = eDirection == EDirection.Front ? Color.red : Color.blue;

            Debug.DrawLine(boxCenter + Vector3.left * boxSize.x * 0.5f, boxCenter + Vector3.right * boxSize.x * 0.5f, c, 1.0f);
            Debug.DrawLine(boxCenter + Vector3.up * boxSize.y * 0.5f, boxCenter + Vector3.down * boxSize.y * 0.5f, c, 1.0f);
            Debug.DrawLine(boxCenter + new Vector3(-boxSize.x, boxSize.y) * 0.5f, boxCenter + new Vector3(boxSize.x, -boxSize.y) * 0.5f,c, 1.0f);
            Debug.DrawLine(boxCenter + new Vector3(boxSize.x, boxSize.y) * 0.5f, boxCenter + new Vector3(-boxSize.x, -boxSize.y) * 0.5f, c, 1.0f);
#endif
            
            // hit이 있다면 벽에 끼는 자리라는 뜻
            if (hit) return false;
            
            // 밑에 땅은 있는지 체크
            hit = Physics2D.BoxCast(boxOrigin, guardian.Physics.TerrainSensor.bounds.size, 0, Vector2.down, 0.1f, groundLayer);

            if (!hit) return false;
            
            guardian.transform.position  = movePoint;
            guardian.LookTarget();
            return true;
        }
    }
}
