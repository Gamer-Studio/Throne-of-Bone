using System.Collections.Generic;
using TMPro;
using ToB.Core;
using ToB.UI;
using ToB.UI.NPC_Dialog;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ToB.Entities.NPC
{
    public class SquareNPC:NPCBase
    {
        enum SquareNPCState
        {
            Phase1,
            Phase2
        }
        
        [SerializeField] private DialogStringOnlySO dialog;
        [SerializeField] private DialogStringOnlySO dialogAccept;
        [SerializeField] private DialogStringOnlySO dialogIgnore;
        
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] SquareNPCState state;

        [SerializeField] private GameObject selectionObj;

        protected override void Awake()
        {
            base.Awake();
            selectionObj.SetActive(false);
        }

        public override void Interact()
        {
            base.Interact();
            state = SquareNPCState.Phase1;
            dialogQueue = new Queue<string>(dialog.Dialogs);
        }

        
        
        public override void ProcessNext()
        {
            base.ProcessNext();

            if (processed)
            {
                processed = false;
                return;
            }
            
            if (dialogQueue.Count > 0)
            {
                SetText(dialogQueue.Dequeue());
            }
            else
            {
                ProcessPhaseEndAction();
            }
        }

        private void ProcessPhaseEndAction()
        {
            switch (state)
            {
                case SquareNPCState.Phase1:
                    selectionObj.SetActive(true);
                    Selection = selectionObj.GetComponent<DialogSelection>();
                    break;
                case SquareNPCState.Phase2:
                    DialogManager.Instance.CancelDialog();
                    break;
                default:
                    break;
            }
            
            state = SquareNPCState.Phase2;
        }

        public void Accept()
        {
            dialogQueue = new Queue<string>(dialogAccept.Dialogs);
            selectionObj.SetActive(false);
            Selection = null;
            
            ProcessNext();
        }

        public void Ignore()
        {
            dialogQueue = new Queue<string>(dialogIgnore.Dialogs);
            selectionObj.SetActive(false);
            Selection = null;
            
            ProcessNext();
        }
    }
}