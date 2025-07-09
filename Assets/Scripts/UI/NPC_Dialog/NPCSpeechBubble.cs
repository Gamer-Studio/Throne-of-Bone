using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToB
{
    public class NPCSpeechBubble : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform InteractShortcutIcon;
        
        RectTransform thisRectTransform;

        bool needReposition = true;
        private void Awake()
        {
            thisRectTransform = GetComponent<RectTransform>();
        }

        public void SetText(string text)
        {
            this.text.text = text;
            
            // 단축키 표시를 위한 강제 갱신이 많습니다
            // 이것 때문에 렉걸릴 시 NPC용 줌인줌아웃할 때 플레이어 UI를 껐다켰다 할 필요가 있지만 일단 보류합니다
            LayoutRebuilder.ForceRebuildLayoutImmediate(thisRectTransform);
            Canvas.ForceUpdateCanvases();   
            
            RePositionShortcutIcon();


        }

        private void RePositionShortcutIcon()
        {
            Vector3[] corners = new Vector3[4];
            thisRectTransform.GetWorldCorners(corners);
            InteractShortcutIcon.position = new Vector3(corners[3].x, corners[3].y, 0);
        }
    }
}
