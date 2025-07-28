using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Teleporter_ByInteract : FieldObjectProgress, IInteractable
    {
        
        [SerializeField] private Transform teleporterPos;
        [SerializeField] private TextMeshProUGUI interactionText;
        public bool IsInteractable { get; set; }

        public override void OnLoad()
        { 
            interactionText.text = "";
            IsInteractable = true;
        }
        public void Interact()
        {
            StageManager.Instance.player.TPTransform = teleporterPos;
            StageManager.Instance.player.TeleportByObject();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactionText.text = "이동하기";
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