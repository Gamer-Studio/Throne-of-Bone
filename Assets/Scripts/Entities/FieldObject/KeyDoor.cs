using Newtonsoft.Json.Linq;
using TMPro;
using ToB.IO;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class KeyDoor: FieldObjectProgress, IInteractable
    {
        private readonly int IsOpened = Animator.StringToHash("IsOpened");
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] public SpriteRenderer DoorSR;
        [SerializeField] public Canvas Infocanvas;
        [SerializeField] public Collider2D DoorCollider;
        [SerializeField] public Animator animator;
        public bool IsInteractable { get; set; }
        public bool isOpened;
        private ObjectAudioPlayer audioPlayer;
        
        #region SaveLoad

        private void OnEnable()
        {
            IsInteractable = true;
            if (audioPlayer == null) audioPlayer = GetComponent<ObjectAudioPlayer>();
        }

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isOpened = json.Get(nameof(isOpened), isOpened);
            
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
            ApplyDoorStates(isOpened);
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
                isOpened = true;
                audioPlayer.Play("Part_Assembly_30");
                ApplyDoorStates(isOpened);
            }
            else
            {
                interactionText.text = "X";
                interactionText.color = Color.red;
            }
        }

        private void ApplyDoorStates(bool _opened)
        {
            interactionText.text = "";
            IsInteractable = !_opened;
            //DoorSR.enabled = !_opened;
            DoorCollider.enabled = !_opened;
            Infocanvas.enabled = !_opened;
            animator.SetBool(IsOpened, _opened);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "F";
                interactionText.color = Color.white;
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