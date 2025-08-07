using System;
using System.Collections;
using ToB.Entities.Buffs;
using ToB.Entities.Projectiles;
using ToB.Player;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ToxicBone : Projectile
    {
        [Header("기본 컴포넌트")]
        [field:SerializeField] public LinearMovement LinearMovement { get; private set; }
        [field:SerializeField] public SimpleRotate SimpleRotate { get; private set; }
        [field:SerializeField] public GameObject EffectPrefab { get; private set; }
        
        [Header("속성 설정")]
        [SerializeField] float baseDamage = 10f;
        //[SerializeField] float dotDamage = 5f;
        [SerializeField] float maxLifeTime = 10f;   // 아무리 길어도 이 시간
                
        // 
        [Tooltip("효과를 입힐 대상"), SerializeField] private LayerMask targetLayers;
        [Tooltip("단순 소멸할 지형"), SerializeField] private LayerMask terrainLayers;

        private Coroutine lifeCoroutine;
        private ObjectAudioPlayer audioPlayer;
        private void Awake()
        {
            if(!LinearMovement)
                LinearMovement = GetComponent<LinearMovement>();
            if(!SimpleRotate)
                SimpleRotate = GetComponent<SimpleRotate>();
            if(!audioPlayer)
                audioPlayer = gameObject.AddComponent<ObjectAudioPlayer>();

            Team = Team.Enemy;
        }

        private void Start()
        {
            targetLayers = LayerMask.GetMask("Player");
            terrainLayers = LayerMask.GetMask("Ground");
            
        }

        private void OnEnable()
        {
            lifeCoroutine = StartCoroutine(ReturnAtLifeTime());
        }

        IEnumerator ReturnAtLifeTime()
        {
            yield return new WaitForSeconds(maxLifeTime);
            gameObject.Release();
        }

        private void Reset()
        {
            LinearMovement = GetComponent<LinearMovement>();
            SimpleRotate = GetComponent<SimpleRotate>();
            
            targetLayers = LayerMask.GetMask("Player");
            terrainLayers = LayerMask.GetMask("Ground");
            
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // 중독 가능한 개체들
            if (targetLayers.Contains(other))
            {
                other.GetComponent<PlayerCharacter>().Damage(baseDamage, this);
                audioPlayer.Play("Bite_03");
                if (other.TryGetComponent<BuffController>(out var buffs))
                {
                    buffs.Apply(Buff.Poison, new BuffInfo(2, 3), true);
                }
                HandleCollide();
            }
            
            if (terrainLayers.Contains(other))
            {
                HandleCollide();
            }
        }

        void HandleCollide()
        {
            if(lifeCoroutine != null) StopCoroutine(lifeCoroutine);
            GameObject effectObj = EffectPrefab.Pooling();
            effectObj.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
