using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ToB.UI.NPC_Dialog
{
    public class DialogSelection:MonoBehaviour
    {
        // 일단은 첫 버튼을 onenable시 포커싱하는 기능만담당합니다
        // 하이어라키상 첫번째 버튼과 같은 위치에
        
        [SerializeField] private Button firstButton;
        [SerializeField] private RectTransform pointer;

        private GameObject current;
        private Button currentButton;
        private void Awake()
        {
            firstButton = GetComponentInChildren<Button>();
        }

        private void OnEnable()
        {
            firstButton.Select();
            LayoutRebuilder.ForceRebuildLayoutImmediate(firstButton.transform as RectTransform);
            
            current = firstButton.gameObject;
            currentButton = current.GetComponent<Button>();
            
            SetPointerNextToButton();
        }

        private void Update()
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            
            if(!selected) currentButton.Select();
            
            // update지만 선택 버튼을 바꿀 때 일어나는 단발성 조작이라서 GetComponent를 과감하게 사용했습니다
            else if (selected != current)
            {
                current = selected;
                currentButton = current.GetComponent<Button>();
                SetPointerNextToButton();
            }
        }

        public void SetPointerNextToButton()
        {
            RectTransform targetRect = current.GetComponent<RectTransform>();
            Vector3 rectPos = targetRect.position;
            pointer.position = rectPos + new Vector3(-2.5f, 0);
        }

        public void Process()
        {
            currentButton.onClick.Invoke();
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}