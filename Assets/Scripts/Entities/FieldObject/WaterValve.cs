using System.Collections;
using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class WaterValve : FieldObjectProgress
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
        private void Awake()
        {
           
        }

        private void SetWaterSize()
        {
            WaterMask.transform.localScale = new Vector3(Width * 2, Height, 1);
            waterSpriteTransform.localScale = new Vector3(Width , Height, 1);
            ValveCloseTargetY = -Height;
            ValveOpenTargetY = Height;
        }
        
        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isValveActivated = json.Get(nameof(isValveActivated), false);
        }

        public override void OnLoad()
        {
            SetWaterSize();
            InitPosition();
        }
        
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Add(nameof(isValveActivated), isValveActivated);
            return json;
        }
        
        #endregion
       
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
                // 벨브 비활성화 = 벨브를 열기 = 물이 나오기 = 수위 상승
            }
            else
            {
                ActivateValve();
                // 벨브 활성화 = 벨브 잠그기 = 물이 안 나옴 = 수위 하락
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
            LinkedLever.interactionText.text = "작동 중";
            Vector3 startPos = Vector3.zero;
            Vector3 targetPos = new Vector3(waterSpriteTransform.localPosition.x, ValveCloseTargetY,
                waterSpriteTransform.localPosition.z);
            waterSpriteTransform.localPosition = startPos;
            
                while (Vector3.Distance(waterSpriteTransform.localPosition, targetPos) > 0.01f)
                {
                    waterSpriteTransform.localPosition = Vector3.MoveTowards(waterSpriteTransform.localPosition,
                        targetPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                LinkedLever.IsInteractable = true;
                LinkedLever.UpdateLeverText();
                Debug.Log("밸브 잠그기 완료");
                ValveOpenCoroutine = null;
        }
        
        private IEnumerator ValveOpen()
        {
            isValveActivated = false;
            LinkedLever.IsInteractable = false;
            LinkedLever.interactionText.text = "작동 중";
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
            LinkedLever.UpdateLeverText();
            Debug.Log("밸브 열기 완료");
            ValveCloseCoroutine = null;
        }

    }
}
