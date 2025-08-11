using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.Entities.Skills;
using ToB.IO;
using ToB.IO.SubModules.SavePoint;
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
                var savePointModule = SAVE.Current.SavePoints;
                var pointData = new SavePointData(StageIndex, RoomIndex, room.bonfires.FindIndex(v => v == this));
                
                savePointModule.lastSavePoint = savePointModule.activeSavePoints.FindIndex(v => v.Equals(pointData));
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
            DebugSymbol.ETC.Log("화톳불 발견");
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
            StageManager.Save(this);
            UIManager.Instance.toastUI.Show("저장이 완료되었습니다.");
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
