using Newtonsoft.Json.Linq;
using TMPro;
using ToB.IO;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class KeyDoor: FieldObjectProgress, IInteractable
    {
        [SerializeField] public TMP_Text interactionText;
        public bool IsInteractable { get; set; }
        public bool isOpened;
        private ObjectAudioPlayer audioPlayer;
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
            audioPlayer = GetComponent<ObjectAudioPlayer>();
        }

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isOpened = json.Get(nameof(isOpened), isOpened);;
            
        }
        
        public override void OnLoad()
        {
            InitState();
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(isOpened)] = isOpened;
            return json;
        }
        #endregion
        private void InitState()
        {
            interactionText.text = "";
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
            if (ToB.Core.ResourceManager.Instance.IsPlayerHaveEnoughMasterKey())
            {
                ToB.Core.ResourceManager.Instance.UseMasterKey();
                OpenedDoorProcess();
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
            audioPlayer.Play("Part_Assembly_30");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "\nF";
                interactionText.color = Color.black;
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