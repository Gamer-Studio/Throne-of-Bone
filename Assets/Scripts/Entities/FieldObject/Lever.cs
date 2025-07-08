using Newtonsoft.Json.Linq;
using TMPro;
using ToB.IO;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Entities.FieldObject
{
    public class Lever : FieldObjectProgress, IInteractable
    {
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] public SpriteRenderer LeverSR;
        [SerializeField] public Sprite[] sprites; // [ 0: off, 1: on]
        [Header("Events : 여기에 드래그해서 발동할 메서드를 호출해 주세요")]
        [SerializeField] public UnityEvent<bool> onLeverInteract;
        public bool IsInteractable { get; set; }
        public bool isLeverActivated;
        
        #region SaveLoad
        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isLeverActivated = json.Get(nameof(isLeverActivated), false);
            IsInteractable = json.Get(nameof(IsInteractable), true);
        }
        
        public override void OnLoad()
        {
            interactionText.text = "";
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Add(nameof(isLeverActivated), isLeverActivated);
            json.Add(nameof(IsInteractable), IsInteractable);
            return json;
        }
      
        #endregion

        /// <summary>
        /// OFF였으면 On으로, On이었으면 Off로
        /// </summary>
        public void Interact()
        {
            isLeverActivated = !isLeverActivated;
            LeverSR.sprite = sprites[isLeverActivated ? 1 : 0];
            UpdateLeverText();
            if (IsInteractable)
            {
                onLeverInteract?.Invoke(isLeverActivated);
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