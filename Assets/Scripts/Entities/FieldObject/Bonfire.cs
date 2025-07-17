using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.IO;
using ToB.Scenes.Stage;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities.FieldObject
{
    public class Bonfire : FieldObjectProgress, IInteractable
    {
        public bool isDiscovered;
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] public Transform TPTransform;
        [SerializeField] public GameObject BonfireUIPanel;
        public bool IsInteractable { get; set; }
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject TPPanel;
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
            animator.SetBool("IsDiscovered", isDiscovered);
            BonfireUIPanel.SetActive(false);
        }
        

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isDiscovered = json.Get(nameof(isDiscovered), isDiscovered);;
        }

        public override void OnLoad()
        {
            
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Add(nameof(isDiscovered), isDiscovered);
            return json;
        }


        #endregion
        
        public void Interact()
        {
            AudioManager.Play("fntgm_magic_fire_08",AudioType.Effect);
            StageManager.Instance.player.stat.Hp = StageManager.Instance.player.stat.maxHp;
            if(!isDiscovered) BonfireDiscovered();
            else
            {
                Debug.Log("상호작용 시작");
                BonfireUIPanel.SetActive(true);
                interactionText.text = "";
                StageManager.Instance.ChangeGameState(GameState.Dialog);
                animator.SetBool("IsUsing", true);
            }
        }

        private void BonfireDiscovered()
        {
            isDiscovered = true;
            animator.SetBool("IsDiscovered", isDiscovered);
            Debug.Log("화톳불 발견");
            interactionText.text = "F : 쉬어가기";
            // 이후 TP포인트에 추가, 지도에 추가 등등
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                
                interactionText.text = isDiscovered ? "F : 쉬어가기" : "F : 발견하기";
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                BonfireUIPanel.SetActive(false);
                interactionText.text = "";
            }
        }
        
        #region ButtonAction

        public void Save()
        {
            Debug.Log("세이브 진행");
        }

        public void TeleportPointSelected()
        {
            Debug.Log("티피합니다!");
            CloseTPPanel();
            LeaveBonfire();
        }

        public void OpenTPPanel()
        {
            TPPanel.SetActive(true);
        }

        public void CloseTPPanel()
        {
            TPPanel.SetActive(false);
        }

        public void LeaveBonfire()
        {
            BonfireUIPanel.SetActive(false);
            StageManager.Instance.ChangeGameState(GameState.Play);
            animator.SetBool("IsUsing", false);
        }
        
        #endregion
    }
}
