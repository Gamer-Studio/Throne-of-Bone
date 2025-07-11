
using ToB.Entities;
using UnityEngine;

namespace ToB
{
    public class ParticleLifePooling : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;

        private void Reset()
        {
            ps = GetComponent<ParticleSystem>();
        }

        private void OnEnable()
        {
            ps.Play();
        }

        private void Update()
        {
            if (!ps.IsAlive(true))
            {
                gameObject.Release();
            }
        }
    }
}
