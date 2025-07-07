using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.Entities.Obstacle
{
    public class KeyDoor: MonoBehaviour, IInteractable
    {
        [SerializeField] public TMP_Text interactionText;
        //[SerializeField] public Image keySprite;
        public bool IsInteractable { get; set; }
        public bool isOpened;
        private void Awake()
        {
            IsInteractable = true;
            isOpened = false;
            // 상호작용 가능 여부, 상호작용 가능 여부는 추후 세이브-로드에서 받아올 것
            InitState();
            interactionText.text = "";
            //keySprite.enabled = false;
        }

        private void InitState()
        {
            if (isOpened)
            {
                IsInteractable = false;
                gameObject.SetActive(false);
            }
            else
            {
                IsInteractable = true;
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 문과의 상호작용. 열쇠가 있으면 문 오브젝트를 Destroy 하는 방식이 좋을지 SetActive(false)하는게 나을지는
        /// 진행도 저장-불러오기를 어떻게 하는지 그 방식을 보고 정해야 할 것 같아요.
        /// </summary>
        public void Interact()
        {
            Debug.Log("Interact");
            if (ToB.Core.ResourceManager.Instance.IsPlayerHaveEnoughMasterKey())
            {
                ToB.Core.ResourceManager.Instance.UseMasterKey();
                OpenedDoorProcess();
                //Destroy(gameObject);
            }
            else
            {
                interactionText.text = "X";
                interactionText.color = Color.red;
            }
        }

        private void OpenedDoorProcess()
        {
            IsInteractable = false;
            isOpened = true;
            interactionText.text = "";
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "\nF";
                interactionText.color = Color.black;
                //keySprite.enabled = true;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "";
                //keySprite.enabled = false;
            }
        }

  
    }
}