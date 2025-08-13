using System;
using System.Collections;
using TMPro;
using ToB.Core.InputManager;
using ToB.UI;
using UnityEngine;
using UnityEngine.UI;

namespace ToB
{
    public class NPCSpeechBubble : MonoBehaviour
    {
        [SerializeField] private GameObject rootCanvasObject;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI speechText;
        [SerializeField] private RectTransform InteractShortcutIcon;
        
        [SerializeField] RectTransform thisRectTransform;
        [SerializeField] private WorldSpaceUIScaler scaler;
        
        public bool skipFlag;
        private void Reset()
        {
            thisRectTransform = GetComponent<RectTransform>();
        }

        private void Awake()
        {
            if(canvasGroup) canvasGroup.alpha = 0;
            if(scaler) scaler.ScaleChangedAction += RebuildLayout;
        }

        public void SetText(string text)
        {
            this.speechText.text = text;
            
            // 단축키 표시를 위한 강제 갱신이 많습니다
            // 이것 때문에 렉걸릴 시 NPC용 줌인줌아웃할 때 플레이어 UI를 껐다켰다 할 필요가 있지만 일단 보류합니다
            RebuildLayout();
        }

        private void RePositionShortcutIcon()
        {
            Vector3[] corners = new Vector3[4];
            thisRectTransform.GetWorldCorners(corners);
            InteractShortcutIcon.position = new Vector3(corners[3].x, corners[3].y, 0);
        }
        
        public IEnumerator SetTextLetterByLetter(string text)
        {
            TOBInputManager.Instance.anyInteractionKeyAction += Skip;
            speechText.text = "";
            yield return null;
            canvasGroup.alpha = 1;

            string textBuffer = "";
            bool metTag = false;
            
            foreach (char letter in text)
            {
                if (letter == '<')
                {
                    metTag = true;
                }

                if (metTag)
                {
                    if (letter != '>')
                    {
                        textBuffer += letter;
                    }
                    else
                    {
                        textBuffer += letter;
                        speechText.text += textBuffer;
                        textBuffer = "";
                        metTag = false;
                    }
                    continue;
                }
                
                speechText.text += letter;
                

                if (skipFlag)
                {
                    speechText.text = text;
                    RebuildLayout();
                    TOBInputManager.Instance.anyInteractionKeyAction -= Skip;
                    skipFlag = false;
                    yield break;  
                }
                RebuildLayout();
                yield return new WaitForSeconds(0.05f);
            }
            
            skipFlag = false;
            TOBInputManager.Instance.anyInteractionKeyAction -= Skip;
        }

        private void Skip()
        {
            skipFlag = true;
        }

        public void ActiveBubbleRoot(bool activeFlag)
        {
            if (!activeFlag) canvasGroup.alpha = 0;
            rootCanvasObject.SetActive(activeFlag);
        }

        void RebuildLayout()
        {
            if(thisRectTransform) LayoutRebuilder.ForceRebuildLayoutImmediate(thisRectTransform);
            Canvas.ForceUpdateCanvases();   
            RePositionShortcutIcon();
        }

        private void OnDestroy()
        {
            if(scaler) scaler.ScaleChangedAction -= RebuildLayout;
        }
    }
}
