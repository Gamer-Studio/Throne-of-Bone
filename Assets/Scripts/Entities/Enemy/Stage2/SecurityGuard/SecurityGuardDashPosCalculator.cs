using UnityEngine;

namespace ToB.Entities
{
    public class SecurityGuardDashPosCalculator : MonoBehaviour
    {
        [SerializeField] private Enemy root;
        [SerializeField] private BoxCollider2D boxCollider;
        [SerializeField] private LayerMask groundLayer;
        private float dashDistance = 0.6f - (-1.065f); // 경비 대원 애니메이션 Attack 클립 참고

        public void Reposition()
        {
            Vector2 origin = (Vector2)boxCollider.bounds.center + boxCollider.bounds.extents.x / 2 * root.LookDirectionHorizontal;
            RaycastHit2D hit = Physics2D.Raycast(origin, root.LookDirectionHorizontal, dashDistance,groundLayer);

            if (hit)
            {
                float repositionX = dashDistance - hit.distance;
                root.transform.position -= new Vector3(repositionX, 0, 0);
            }
        }
    }
}
