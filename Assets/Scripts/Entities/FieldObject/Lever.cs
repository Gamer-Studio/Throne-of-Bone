using System;
using Newtonsoft.Json.Linq;
using TMPro;
using ToB.IO;
using ToB.Utils;
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
        [SerializeField] public Animator animator;
        public bool IsInteractable { get; set; }
        [SerializeField] public bool isLeverActivated;
        private ObjectAudioPlayer audioPlayer;
        
        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isLeverActivated = json.Get(nameof(isLeverActivated), isLeverActivated);
            IsInteractable = json.Get(nameof(IsInteractable), true);
            animator.SetBool("IsActivated", isLeverActivated);
            audioPlayer = GetComponent<ObjectAudioPlayer>();
        }
        
        public override void OnLoad()
        {
            LeverSR.sprite = sprites[isLeverActivated ? 1 : 0];
            //interactionText.text = "";
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(isLeverActivated)] = isLeverActivated;
            json[nameof(IsInteractable)] = IsInteractable;
            return json;
        }
      
        #endregion

        /// <summary>
        /// OFF였으면 On으로, On이었으면 Off로
        /// </summary>
        public void Interact()
        {
            if (IsInteractable)
            {
                isLeverActivated = !isLeverActivated;
                LeverStateUpdate();
                onLeverInteract?.Invoke(isLeverActivated);
                audioPlayer.Play("Part_Assembly_01");
            }
        }
        public void LeverStateUpdate()
        {
            animator.SetBool("IsActivated", isLeverActivated);
            UpdateLeverText();
        }
        

        public void UpdateLeverText()
        {
            interactionText.text = !isLeverActivated ? "F : 켜기" : "F : 끄기";
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && IsInteractable)
            {
                interactionText.enabled = true;
                UpdateLeverText();
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.enabled = false;
                interactionText.text = "";
            }
        }
        
    }
}