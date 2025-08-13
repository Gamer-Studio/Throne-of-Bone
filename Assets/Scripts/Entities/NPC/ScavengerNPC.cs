using System.Collections.Generic;
using ToB.Core;
using ToB.UI.NPC_Dialog;
using UnityEngine;

namespace ToB.Entities.NPC
{
    public class ScavengerNPC:NPCBase
    {
        [Header("Scavenger NPC")]
        [SerializeField] private DialogStringOnlySO dialogEntry;
        [SerializeField] private DialogStringOnlySO dialogShopSelected;
        [SerializeField] private DialogStringOnlySO dialogQuitSelected;
        
        [SerializeField] private GameObject selectionObj;
        

        enum ScavengerNPCState
        {
            Entry,
            ShopSelected,
            Exit
        }
        
        ScavengerNPCState state;
        public override void Interact()
        {
            base.Interact();
            dialogQueue = new Queue<string>(dialogEntry.Dialogs);
            state = ScavengerNPCState.Entry;
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
                case ScavengerNPCState.Entry:
                    selectionObj.SetActive(true);
                    Selection = selectionObj.GetComponent<DialogSelection>();
                    break;
                case ScavengerNPCState.ShopSelected:
                    DialogManager.Instance.CancelDialog();
                    break;
                case ScavengerNPCState.Exit:
                    DialogManager.Instance.CancelDialog();
                    break;
                default:
                    break;
            }
            
        }

        public void SelectShop()
        {
            dialogQueue = new Queue<string>(dialogShopSelected.Dialogs);
            state = ScavengerNPCState.ShopSelected;
           
            selectionObj.SetActive(false);
            Selection = null;
            ProcessNext();
            
        }

        public void SelectExit()
        {
            dialogQueue = new Queue<string>(dialogQuitSelected.Dialogs);
            state = ScavengerNPCState.Exit;
            
            selectionObj.SetActive(false);
            Selection = null;
            ProcessNext();
        }
    }
}