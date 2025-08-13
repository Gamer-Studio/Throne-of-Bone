using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Door : FieldObjectProgress
    {
        private static readonly int IsOpened = Animator.StringToHash("IsOpened");

        enum DoorState
        {
            Open,
            Close
        }
        
        private Vector3 openPos;
        private Vector3 closedPos;
        
        [Header("컴포넌트")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private BoxCollider2D boxCollider2D;
        
        [Header("스프라이트 적용 이전 기능")]
        [SerializeField, ReadOnly] DoorState doorState;
        [SerializeField] private float heightToOpen;
        [SerializeField] private float doorMoveSpeed = 10;
        [SerializeField] private bool openAtStart;
        
        
        
        public bool IsOpen => doorState == DoorState.Open;
        
        private void Awake()
        {
            // closedPos = transform.localPosition;
            // openPos = closedPos + Vector3.up * heightToOpen;
            //
            // if (openAtStart)
            // {
            //     transform.localPosition = openPos;
            //     doorState = DoorState.Open;
            // }
            Open();
        }

        private void Reset()
        {
            doorState = DoorState.Close;
        }

        public void Open()
        {
            //StartCoroutine(DoorOpenCoroutine());
            animator.SetBool(IsOpened, true);
            boxCollider2D.enabled = false;
            doorState = DoorState.Open;
        }
        public void Close()
        {
            animator.SetBool(IsOpened, false);
            boxCollider2D.enabled = true;
            doorState = DoorState.Close;
        }

        IEnumerator DoorOpenCoroutine()
        {
            while (transform.localPosition.y < openPos.y)
            {
                yield return null;
                transform.localPosition += Vector3.up * (Time.deltaTime * doorMoveSpeed);
            }
            transform.localPosition = openPos;
            doorState = DoorState.Open;
            
        }
        IEnumerator DoorCloseCoroutine()
        {
            while (transform.localPosition.y > closedPos.y)
            {
                yield return null;
                transform.localPosition -= Vector3.up * (Time.deltaTime * doorMoveSpeed);
            }
            transform.localPosition = closedPos;
            doorState = DoorState.Close;
        }
    }
}
