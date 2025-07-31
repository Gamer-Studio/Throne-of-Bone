using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.Entities.Skills;
using ToB.IO;
using ToB.Scenes.Stage;
using ToB.UI;
using ToB.Utils;
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
        private ObjectAudioPlayer audioPlayer;
        [SerializeField] private Animator animator;
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
            BonfireUIPanel.SetActive(false);
            audioPlayer = GetComponent<ObjectAudioPlayer>();
        }
        

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isDiscovered = json.Get(nameof(isDiscovered), isDiscovered);
        }

        public override void OnLoad()
        {
            animator.SetBool("IsDiscovered", isDiscovered);
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(isDiscovered)] = isDiscovered;
            return json;
        }


        #endregion
        
        public void Interact()
        {
            audioPlayer.Play("fntgm_magic_fire_08");
            StageManager.Instance.player.stat.HealtoFullHp();
            if(!isDiscovered) BonfireDiscovered();
            else
            {
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
            var player = SAVE.Current.Player;
            
            SAVE.Current.SavePoints.UpdateSavePoint(this);
            SAVE.Current.Save();
        }

        public void TeleportPointSelected()
        {
            Debug.Log("티피합니다!");
            TPPanelToggle();
            LeaveBonfire();
        }

        public void TPPanelToggle()
        {
            UIManager.Instance.wideMapUI.TPPanelToggle();
        }
        
        public void LeaveBonfire()
        {
            BonfireUIPanel.SetActive(false);
            interactionText.text = "F : 쉬어가기";
            StageManager.Instance.ChangeGameState(GameState.Play);
            animator.SetBool("IsUsing", false);
        }
        
        #endregion
    }
}
