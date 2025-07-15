using System;
using System.Collections.Generic;
using Cinemachine;
using ToB.Core;
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
        [SerializeField] private RectTransform DialogRoot;  // Content Fitter와 연게된 보조 레이아웃(ex-단축키 아이콘) 배치 강제 리프레시용
        
        /// <summary>
        /// NPC와 대화할 때 바뀔 줌 사이즈입니다.
        /// </summary>
        [field:SerializeField] public float ZoomSize { get; private set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; private set; }
        
        protected Queue<string> dialogQueue;
        
        protected bool processed = false;

        protected virtual void Awake()
        {
            IsInteractable = true;
            DialogPanel.gameObject.SetActive(false);
        }

        protected virtual void Reset()
        {
            Bubble = GetComponent<NPCSpeechBubble>();
            DialogPanel = GetComponentInChildren<UIPanelBase>();
        }
        protected void SetText(string text)
        {
            Bubble.SetText(text);
        }

        public virtual void Interact()
        {
            if (StageManager.Instance.player.transform.position.x < transform.position.x)
            {
                Sprite.flipX = true;
            }
            DialogManager.Instance.StartDialogWith(this);
        }

        public virtual void ProcessNext()
        {
            if (Selection)
            {
                Selection.Process();
                Selection = null;
                processed = true;
            }
        }
    }
}
