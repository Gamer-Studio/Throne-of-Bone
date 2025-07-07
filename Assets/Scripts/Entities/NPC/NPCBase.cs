using System;
using ToB.Entities.Obstacle;
using UnityEngine;

namespace ToB.Entities.NPC
{
    public abstract class NPCBase : MonoBehaviour, IInteractable
    {
        public bool IsInteractable { get; set; }

        private void Awake()
        {
            IsInteractable = true;
        }


        public abstract void Interact();
    }
}
