using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.IO;
using ToB.Utils;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities.FieldObject
{
    public class Chest_Big : FieldObjectProgress, IInteractable
    {
        [SerializeField] public int gold;
        [SerializeField] public int mana;
        [SerializeField] public TMP_Text interactionText;
        [SerializeField] private Animator animator;
        
        private ObjectAudioPlayer audioPlayer;
        public bool IsInteractable { get; set; }
        private bool IsOpened;

        private void Awake()
        {
            
        }

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            IsOpened = json.Get(nameof(IsOpened), IsOpened);
            if (!audioPlayer) audioPlayer = GetComponent<ObjectAudioPlayer>();
            IsInteractable = !IsOpened;
            animator.SetBool("IsOpened", IsOpened);
        }
        
        public override void OnLoad()
        {
            
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(IsOpened)] = IsOpened;
            return json;
        }
        public void Interact()
        {
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Gold, gold, transform);
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Mana, mana, transform);
            IsOpened = true;
            IsInteractable = false;
            interactionText.text = "";
            audioPlayer.Play("env_chest_open_01");
            animator.SetBool("IsOpened", IsOpened);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && IsInteractable)
            {
                interactionText.text = "F : 열기";
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