using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class PoolRetrieveByLife : MonoBehaviour
    {
        public float lifeTime;
        float enableTime;
        private void OnEnable()
        {
            enableTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - enableTime > lifeTime)
                gameObject.Release();
        }
    }
}
