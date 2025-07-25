using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PressurePlate : FieldObjectProgress
    {
        public enum PlateType
        {
            Stone,
            Wood
        }
        
        [Header("Plate Type에 따른 스프라이트")]
        [SerializeField] private PlateType plateType;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites; // 0~1 Stone, 2~3 Wood
        
        [Header("인터렉션 영역")]
        [SerializeField] private float DetectionWidth;
        [SerializeField] private bool IsActivated;
        [SerializeField] private bool IsCleared;
        [SerializeField] private BoxCollider2D boxCollider;
        // 설명 : 레이케스트를 해도 될 것 같지만, 간단하게 올라가고 내려가는 것만 감지하고
        // 상자가 여러 개 눌린다거나 깊게 눌린다거나 하는 게 없으니 콜라이더 기반 충돌로 해결.
        private float initialWidth;
        private int objectCount;

        /// <summary>
        /// 클리어 조건 만족 시 호출. 한번 호출되면 바뀔 일이 없지 싶어요.
        /// 호출은 아마도 새로 만들 RoomPuzzleSetter 오브젝트에서 관리할 듯 싶어요.
        /// </summary>
        public void Clear()
        {
            IsCleared = true;
            SetPlateSprite(plateType, IsCleared);
            objectCount = 0;
            SetColliderWidth(0);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsCleared &&(other.CompareTag("Player") || other.CompareTag("Box")))
            {
                objectCount++;
                IsActivated = objectCount > 0;
                // 소리 여기서 재생 if(IsActivated) Play~
                SetPlateSprite(plateType, IsActivated);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsCleared &&(other.CompareTag("Player") || other.CompareTag("Box")))
            {
                objectCount--;
                IsActivated = objectCount > 0;
                // 소리 여기서 재생 if(!IsActivated) Play~
                SetPlateSprite(plateType, IsActivated);
            }
        }
        
        private void SetPlateSprite(PlateType type, bool isActivated = false)
        {
            switch (type)
            {
                case PlateType.Stone:
                    spriteRenderer.sprite = isActivated?  sprites[1] : sprites[0];
                    break;
                case PlateType.Wood:
                    spriteRenderer.sprite = isActivated?  sprites[3] : sprites[2];
                    break;
            }
        }

        /// <summary>
        /// 콜라이더의 가로 크기 설정 (0 ~ 1).
        /// 1 = 발판의 너비. 즉 상자가 조금만 닿아도 발동
        /// ~ 0 = 감지 영역이 작아질수록 상자를 더 깊이 넣어야 발동
        /// 0을 넣으면 콜라이더를 꺼버려서 감지가 불가능
        /// </summary>
        /// <param name="width"></param>
        private void SetColliderWidth(float width)
        {
            Debug.Log(width);
            if (width == 0)
            {
                boxCollider.enabled = false;
            }
            else
            {
                boxCollider.enabled = true;
                boxCollider.size = new Vector2(initialWidth * width, boxCollider.size.y);
            }
        }

        #region SaveLoad
        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            IsCleared = json.Get(nameof(IsCleared), false);
        }
        
        public override void OnLoad()
        {
            if (!IsCleared)
            {
                objectCount = 0;
                initialWidth = boxCollider.size.x;
                SetColliderWidth(DetectionWidth);
                SetPlateSprite(plateType, IsCleared);
            }
            else Clear();
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(IsCleared)] = IsCleared;
            return json;
        }
        #endregion

    }
}
