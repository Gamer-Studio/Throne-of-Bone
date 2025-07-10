using Newtonsoft.Json.Linq;
using TMPro;
using ToB.Core;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Chest_Big : FieldObjectProgress, IInteractable
    {
        [SerializeField] public int gold;
        [SerializeField] public int mana;
        [SerializeField] public TMP_Text interactionText;
        public bool IsInteractable { get; set; }
        private bool IsOpened;

        private void Awake()
        {
            IsOpened = false;
            IsInteractable = true;
        }

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            IsOpened = json.Get(nameof(IsOpened), IsOpened);
        }
        
        public override void OnLoad()
        {
            IsInteractable = !IsOpened;
            gameObject.SetActive(!IsOpened);
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Add(nameof(IsOpened), IsOpened);
            return json;
        }
        public void Interact()
        {
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Gold, gold, transform);
            Core.ResourceManager.Instance.SpawnResources(InfiniteResourceType.Mana, mana, transform);
            IsOpened = true;
            IsInteractable = false;
            interactionText.text = "";
            gameObject.SetActive(false);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
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