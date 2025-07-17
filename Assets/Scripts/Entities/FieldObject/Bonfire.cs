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
        [SerializeField] public GameObject TempTPPanel;
        public bool IsInteractable { get; set; }
        [SerializeField] private Animator animator;
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
            animator.SetBool("IsDiscovered", isDiscovered);
            TempTPPanel.SetActive(false);
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
                Debug.Log("저장하기 실행");
            }
            animator.SetTrigger("IsActivated");
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
                TempTPPanel.SetActive(true);
                interactionText.text = isDiscovered ? "F : 저장하기" : "F : 쉬어가기";
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                TempTPPanel.SetActive(false);
                interactionText.text = "";
            }
        }
    }
}
