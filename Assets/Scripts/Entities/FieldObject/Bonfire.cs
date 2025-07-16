using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.IO;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities.FieldObject
{
    public class Bonfire : FieldObjectProgress, IInteractable
    {
        public bool isDiscovered;
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] public Transform TPTransform;
        //[SerializeField] public TMP_Text TPText;
        public bool IsInteractable { get; set; }
        [SerializeField] private Animator animator;
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
            animator.SetBool("IsDiscovered", isDiscovered);
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
            if(!isDiscovered) BonfireDiscovered();
            else
            {
                var playerData = SAVE.Current.Player;

                playerData.currentStage = StageIndex;
                playerData.currentRoom = RoomIndex;
                playerData.savedPosition = TPTransform.localPosition + transform.localPosition;
                SAVE.Current.Save();
            }
            animator.SetTrigger("IsActivated");
        }

        private void BonfireDiscovered()
        {
            isDiscovered = true;
            animator.SetBool("IsDiscovered", isDiscovered);
            Debug.Log("화톳불 발견");
            // 이후 TP포인트에 추가, 지도에 추가 등등
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            { 
                interactionText.text = "F : 상호작용";
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
