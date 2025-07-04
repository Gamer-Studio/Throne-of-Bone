using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace ToB.Entities.Obstacle
{
    public class Lever : MonoBehaviour, IInteractable
    {
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] public SpriteRenderer LeverSR;
        [SerializeField] public Sprite[] sprites; // [ 0: off, 1: on]
        public bool isLeverActivated;
        [Header("Events : 여기에 드래그해서 발동할 메서드를 호출해 주세요")]
        [SerializeField] public UnityEvent<bool> onLeverInteract;
        public bool IsInteractable { get; set; }
        // 외부에서 레버의 IsInteractable을 접근?
        private void Awake()
        {
            IsInteractable = true;
            interactionText.text = "";
        }
        
        /// <summary>
        /// OFF였으면 On으로, On이었으면 Off로
        /// </summary>
        public void Interact()
        {
            Debug.Log("interact");
            isLeverActivated = !isLeverActivated;
            LeverSR.sprite = sprites[isLeverActivated ? 1 : 0];
            UpdateLeverText();
            if (IsInteractable)
            {
                onLeverInteract?.Invoke(isLeverActivated);
                Debug.Log($"레버 이벤트 Invoke : {isLeverActivated}");
            }
        }

        private void UpdateLeverText()
        {
            if (!isLeverActivated) interactionText.text = "F : 켜기";
            else interactionText.text = "F : 끄기";
        }



        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                UpdateLeverText();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "";
            }
        }
        
        
    }
}