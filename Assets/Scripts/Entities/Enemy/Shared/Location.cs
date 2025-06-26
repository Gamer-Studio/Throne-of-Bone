using UnityEngine;

namespace ToB
{
    public class Location : MonoBehaviour
    {
        [SerializeField] private float width;
        [SerializeField] private float height;

        public Vector2 GetRandomPosition(bool fixedX = false, bool fixedY = false)
        {
            float x = fixedX ? transform.position.x : Random.Range(transform.position.x - width / 2, transform.position.x + width / 2);
            float y = fixedY ? transform.position.y : Random.Range(transform.position.y - height / 2, transform.position.y + height / 2);
            return new Vector2(x, y);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red.WithAlpha(0.7f);
            Gizmos.DrawCube(transform.position, new Vector3(width, height, 0));
        }
    }
}
