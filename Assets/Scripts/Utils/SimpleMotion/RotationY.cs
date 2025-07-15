using UnityEngine;

namespace ToB
{
    public class RotationY : MonoBehaviour
    {
        public float rotSpeed = 100;
        void Update()
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0);
        }
    }
}
