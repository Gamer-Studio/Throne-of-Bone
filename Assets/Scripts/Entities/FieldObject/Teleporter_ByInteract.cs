using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using ToB.Core;
using ToB.Scenes.Stage;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

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
            AudioManager.Stop(AudioType.Background);
            if (StageManager.Instance.CurrentStageIndex == 1) AudioManager.Play("1.Stage", AudioType.Background);
            else if (StageManager.Instance.CurrentStageIndex == 2) AudioManager.Play("2.Stage", AudioType.Background);
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