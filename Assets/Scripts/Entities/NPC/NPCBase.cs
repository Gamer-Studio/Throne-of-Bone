using System;
using System.Collections.Generic;
using Cinemachine;
using ToB.Core;
using ToB.Entities.Obstacle;
using ToB.Scenes.Stage;
using ToB.UI;
using ToB.UI.NPC_Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.Entities.NPC
{
    public abstract class NPCBase : MonoBehaviour, IInteractable
    {
        public bool IsInteractable { get; set; }
        [field:SerializeField] public UIPanelBase DialogPanel { get; set; }
        protected DialogSelection Selection { get; set; }
        [field:SerializeField] public NPCSpeechBubble Bubble { get; protected set; }
        [SerializeField] private RectTransform DialogRoot;

        protected Queue<string> dialogQueue;
        
        private bool focused = false;
        protected bool processed = false;

        protected virtual void Awake()
        {
            IsInteractable = true;
            DialogPanel.gameObject.SetActive(false);
        }
        protected void SetText(string text)
        {
            Bubble.SetText(text);
        }

        public virtual void Interact()
        {
            StageManager.Instance.ChangeGameState(GameState.UI);
            DialogManager.Instance.StartDialogWith(this);
        }

        public virtual void ProcessNext()
        {
            Debug.Log("process next");
            if (Selection)
            {
                Selection.Process();
                Selection = null;
                processed = true;
            }
        }
    }
}
