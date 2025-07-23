using System;
using System.Collections;
using System.Collections.Generic;
using ToB.Entities.Interface;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PopUpBlade : FieldObjectProgress
    {
        private readonly int IsActivated = Animator.StringToHash("IsActivated");

        /// <summary>
        /// Detect : 일정 범위 안에 플레이어 들어오면 튀어나옴. 나가면 다시 들어감.
        /// Timer : 설정한 시간 간격대로 튀어나오고 들어가기를 반복함.
        /// Disabled : 비활성화된 상태. 데미지 없음!
        /// </summary>
        public enum PopUpBladeState
        {
            Detect,
            Timer,
            Disabled
        }
        
        [SerializeField] private Animator animator;
        [SerializeField] private PopUpBladeState state;
        [SerializeField] private float activeTime;
        [SerializeField] private float safeTime;
        [SerializeField] private float knockBackPower;
        [SerializeField] private float damage;
        
        private Coroutine C_popUpTimer;
        private bool isActivated;
        
        private PolygonCollider2D PolygonCollider;
        private SpriteRenderer SpriteRenderer;
        private Sprite lastSprite;

        private void Awake()
        {
            if(!animator) animator = GetComponent<Animator>();
            if(!PolygonCollider) PolygonCollider = GetComponent<PolygonCollider2D>();
            if(!SpriteRenderer) SpriteRenderer = GetComponent<SpriteRenderer>();
            lastSprite = SpriteRenderer.sprite;
            
            SetState(state);
        }

        private void LateUpdate()
        {
            if (state != PopUpBladeState.Disabled && lastSprite != SpriteRenderer.sprite)
            {
                UpdateCollider();
                lastSprite = SpriteRenderer.sprite;
            }
        }

        private void TimerCoroutine()
        {
            if( C_popUpTimer != null)
            {
                StopCoroutine(C_popUpTimer);
                C_popUpTimer = null;
            }
            C_popUpTimer = StartCoroutine(Timer());
        }

        private IEnumerator Timer()
        {
            while (state == PopUpBladeState.Timer)
            {
                isActivated = true;
                animator.SetBool(IsActivated, true);
                yield return new WaitForSeconds(activeTime);
                isActivated = false;
                animator.SetBool(IsActivated, false);
                yield return new WaitForSeconds(safeTime);
            }
            // 오류방지용 중단코드
            isActivated = false;
            animator.SetBool(IsActivated, false);
            C_popUpTimer = null;
        }
        private void SetState(PopUpBladeState _state)
        {
            this.state = _state;
            
            if (state == PopUpBladeState.Detect)
            {
                animator.SetBool(IsActivated, false);
            }

            else if (state == PopUpBladeState.Timer)
            {
                TimerCoroutine();
            }
            
            else if (state == PopUpBladeState.Disabled)
            {
                DeActivateBlade();
            }
        }
        
        /// <summary>
        /// 레버 등으로 작동을 꺼야 할 때 사용
        /// </summary>
        public void DeActivateBlade()
        {
            if (C_popUpTimer != null) StopCoroutine(C_popUpTimer);
            C_popUpTimer = null;
            isActivated = false;
            animator.SetBool(IsActivated, false);
        }
        public void ActivateBlade()
        {
            isActivated = true;
            animator.SetBool(IsActivated, true);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && isActivated)
            {
                IDamageableExtensions.Damage(other, damage, (IAttacker)this);
                other.KnockBack(knockBackPower, gameObject);
            }
        }

        private void UpdateCollider()
        {
            PolygonCollider.pathCount = 0; // 리셋
            PolygonCollider.pathCount = SpriteRenderer.sprite.GetPhysicsShapeCount();

            for (int i = 0; i < PolygonCollider.pathCount; i++)
            {
                var path = new List<Vector2>();
                SpriteRenderer.sprite.GetPhysicsShape(i, path);
                PolygonCollider.SetPath(i, path);
            }
        }

        public PopUpBladeState GetState()
        {
            return state;
        }

    }
}
