using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Memories;
using ToB.UI;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class MemoryItem : FieldObjectProgress
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] public int memoryItemID;
        public bool IsCollected;
        private ObjectAudioPlayer audioPlayer;

        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            IsCollected = json.Get(nameof(IsCollected), false);
        }

        public override void OnLoad()
        {
            if (audioPlayer == null) audioPlayer = GetComponent<ObjectAudioPlayer>();
            spriteRenderer.enabled = !IsCollected;
        }
        
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(IsCollected)] = IsCollected;
            return json;
        }

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !IsCollected)
            {
                IsCollected = true;
                UIManager.Instance.toastUI.Show("일지를 획득했다!");
                MemoriesManager.Instance.MemoryAcquired(memoryItemID);
                spriteRenderer.enabled = false;
            }
        }
        
    }
}