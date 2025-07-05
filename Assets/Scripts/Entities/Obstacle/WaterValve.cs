using System;
using UnityEngine;
using System.Collections;
using Unity.AppUI.UI;

namespace ToB.Entities.Obstacle
{
    public class WaterValve : MonoBehaviour
    {
        [SerializeField] public Transform waterSpriteTransform;
        [SerializeField] public float moveSpeed; // 물줄기 내려오는 속도
        [SerializeField] public float Height;
        [SerializeField] public float Width;
        private float ValveCloseTargetY; // 물 끊길 때 물 내려가는 위치
        private float ValveOpenTargetY; // 물 틀 때 시작하는 위치
        [SerializeField] public Lever LinkedLever;
        [SerializeField] public RectTransform WaterMask;
        
        public bool isValveActivated;
        
        private Coroutine ValveCloseCoroutine;
        private Coroutine ValveOpenCoroutine;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetWaterSize();
        }

#endif
        private void SetWaterSize()
        {
            WaterMask.transform.localScale = new Vector3(Width * 2, Height, 1);
            waterSpriteTransform.localScale = new Vector3(Width , Height, 1);
            ValveCloseTargetY = -Height;
            ValveOpenTargetY = Height;
        }

        private void Awake()
        {
            isValveActivated = false;
            SetWaterSize();
            InitPosition();
            // 추후 여기서 벨브가 열렸는지 안 열렸는지 Load해 오기
        }
        
        private void InitPosition()
        {
            if (isValveActivated)
            {
                waterSpriteTransform.localPosition = new Vector3(waterSpriteTransform.localPosition.x,
                    ValveOpenTargetY, waterSpriteTransform.localPosition.z);
            }
            else
            {
                waterSpriteTransform.localPosition = Vector3.zero;
            }
        }

        public void LeverInteraction(bool leverState)
        {
            if (!leverState)
            {
                DeactivateValve();
            }
            else
            {
                ActivateValve();
            }
        }

        public void DeactivateValve()
        {
            if (ValveOpenCoroutine == null)
            {
                ValveOpenCoroutine = StartCoroutine(ValveOpen());
            }
        }

        public void ActivateValve()
        {
            if (ValveCloseCoroutine == null)
            {
                ValveCloseCoroutine = StartCoroutine(ValveClose());
            }
        }

        private IEnumerator ValveClose()
        {
            isValveActivated = true;
            LinkedLever.IsInteractable = false;
            Vector3 startPos = Vector3.zero;
            Vector3 targetPos = new Vector3(waterSpriteTransform.localPosition.x, ValveCloseTargetY,
                waterSpriteTransform.localPosition.z);
            waterSpriteTransform.localPosition = startPos;
            
                // 물 내려오기 (이어짐)
                while (Vector3.Distance(waterSpriteTransform.localPosition, targetPos) > 0.01f)
                {
                    waterSpriteTransform.localPosition = Vector3.MoveTowards(waterSpriteTransform.localPosition,
                        targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                LinkedLever.IsInteractable = true;
                Debug.Log("밸브 잠그기 완료");
                ValveOpenCoroutine = null;
        }
        
        private IEnumerator ValveOpen()
        {
            isValveActivated = false;
            LinkedLever.IsInteractable = false;
            Vector3 startPos = new Vector3(waterSpriteTransform.localPosition.x, ValveOpenTargetY,
                waterSpriteTransform.localPosition.z);
            Vector3 targetPos = Vector3.zero;
            waterSpriteTransform.localPosition = startPos;
            
            while (Vector3.Distance(targetPos, waterSpriteTransform.localPosition) > 0.01f)
            {
                waterSpriteTransform.localPosition = Vector3.MoveTowards(waterSpriteTransform.localPosition,
                    targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            LinkedLever.IsInteractable = true;
            Debug.Log("밸브 열기 완료");
            ValveCloseCoroutine = null;
        }

    }
}
