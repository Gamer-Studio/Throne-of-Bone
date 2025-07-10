using System.Collections.Generic;
using ToB.Core;
using UnityEngine;

namespace ToB.Entities.NPC
{
    public class ScavengerNPC:NPCBase
    {
        [Header("Scavenger NPC")]
        [SerializeField] private DialogStringOnlySO _dialog;

        public override void Interact()
        {
            base.Interact();
            dialogQueue = new Queue<string>(_dialog.Dialogs);
        }

        public override void ProcessNext()
        {
            base.ProcessNext();
                
            if (dialogQueue.Count > 0)
            {
                SetText(dialogQueue.Dequeue());
            }
            else
            {
                DialogManager.Instance.CancelDialog();
            }
        }
    }
}