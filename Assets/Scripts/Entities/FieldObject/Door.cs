using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Door : FieldObjectProgress
    {
        enum DoorState
        {
            Open,
            Close
        }
        
        private Vector3 openPos;
        private Vector3 closedPos;
        
        [SerializeField, ReadOnly] DoorState doorState;
        [SerializeField] private float heightToOpen;
        [SerializeField] private float doorMoveSpeed = 10;
        [SerializeField] private bool openAtStart;
        
        public bool IsOpen => doorState == DoorState.Open;
        
        private void Awake()
        {
            closedPos = transform.localPosition;
            openPos = closedPos + Vector3.up * heightToOpen;

            if (openAtStart)
            {
                transform.localPosition = openPos;
                doorState = DoorState.Open;
            }
        }

        private void Reset()
        {
            doorState = DoorState.Close;
        }

        public void Open()
        {
            StartCoroutine(DoorOpenCoroutine());
        }
        public void Close()
        {
            StartCoroutine(DoorCloseCoroutine());
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
