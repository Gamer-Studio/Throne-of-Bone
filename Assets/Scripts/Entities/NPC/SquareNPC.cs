using ToB.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToB.Entities.NPC
{
    public class SquareNPC:NPCBase
    {
        public override void Interact()
        {
            base.Interact();
            Debug.Log("Oh hi!");
        }

        public void OnClickDialogButton()
        {
            DialogManager.Instance.StartDialog(this);
        }
        
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                var raycastResults = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, raycastResults);

                Debug.Log($"Raycast hit {raycastResults.Count} UI elements");

                foreach (var result in raycastResults)
                {
                    Debug.Log("Hit: " + result.gameObject.name);
                }
            }
        }
    }
}