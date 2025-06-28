using System;
using ToB.Player;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ToxicBone : MonoBehaviour
    {
        [Header("기본 컴포넌트")]
        [field:SerializeField] public LinearMovement LinearMovement { get; private set; }
        [field:SerializeField] public SimpleRotate SimpleRotate { get; private set; }
        
        [Header("속성 설정")]
        [SerializeField] float baseDamage = 10f;
        //[SerializeField] float dotDamage = 5f;
        [SerializeField] float maxLifeTime = 10f;   // 아무리 길어도 이 시간
                
        // 
        [Tooltip("효과를 입힐 대상"), SerializeField] private LayerMask targetLayers;
        [Tooltip("단순 소멸할 지형"), SerializeField] private LayerMask terrainLayers;
            
        private void Awake()
        {
            if(!LinearMovement)
                LinearMovement = GetComponent<LinearMovement>();
            if(!SimpleRotate)
                SimpleRotate = GetComponent<SimpleRotate>();
        }

        private void Start()
        {
            targetLayers = LayerMask.GetMask("Player");
            terrainLayers = LayerMask.GetMask("Ground");
            Destroy(gameObject, maxLifeTime);
        }

        private void Reset()
        {
            LinearMovement = GetComponent<LinearMovement>();
            SimpleRotate = GetComponent<SimpleRotate>();
            
            targetLayers = LayerMask.GetMask("Player");
            terrainLayers = LayerMask.GetMask("Ground");
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 중독 가능한 개체들
            if ((targetLayers & 1 << other.gameObject.layer) != 0)
            {
                // other.GetComponent<IHittable>().OnHit();
                // other.GetComponent<IPoisonable>().ApplyPoison();

                other.GetComponent<PlayerCharacter>().Damage(baseDamage, this);
                
                Destroy(gameObject);
            }
            
            if ((terrainLayers & 1 << other.gameObject.layer) != 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
