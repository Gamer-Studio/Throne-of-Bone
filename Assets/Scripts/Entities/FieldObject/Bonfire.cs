using Newtonsoft.Json.Linq;
using TMPro;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Bonfire : FieldObjectProgress, IInteractable
    {
        public bool isDiscovered;
        [SerializeField] public TMP_Text interactionText;
        //[SerializeField] public TMP_Text TPText;
        public bool IsInteractable { get; set; }
        
        #region SaveLoad

        private void Awake()
        {
            IsInteractable = true;
        }
        

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isDiscovered = json.Get(nameof(isDiscovered), false);
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
            Debug.Log("저장 메서드 실행!!!");    
        }

        private void BonfireDiscovered()
        {
            isDiscovered = true;                
            Debug.Log("화톳불 발견"); 
            // 이후 TP포인트에 추가, 지도에 추가 등등
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if(!isDiscovered) BonfireDiscovered();
                interactionText.text = "F : 저장하기";
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
