using System;
using Cinemachine;
using ToB.Entities.Obstacle;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.NPC
{
    public abstract class NPCBase : MonoBehaviour, IInteractable
    {
        public bool IsInteractable { get; set; }

        private bool focused = false;

        private void Awake()
        {
            IsInteractable = true;
        }


        public virtual void Interact()
        {
            if (!focused)
            {
                focused = true;
                StageManager.Instance.ChangeGameState(GameState.UI);
            }
        }
    }
}
