using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.Entities.FieldObject
{
    public class LetterBoxSign : FieldObjectProgress
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI toastText;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GameObject textbox;
        
        
        [SerializeField] private Transform letterBoxPos;
        [SerializeField] private string letter;
        
        [Header("말풍선 관련 : 다음 글자 칠 때까지 기다리는 시간(초)")]
        [SerializeField] private float typingSpeed;
        private Coroutine typingCoroutine;
        public bool IsShowingTextEnd {get; set;}

        private void Awake()
        {
            image = GetComponentInChildren<Image>();
            image.color = Color.black;
            textbox.SetActive(false);
        }
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Typing(letter, letterBoxPos.position);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            IsShowingTextEnd = true;
        }
        
        public void Typing(string message, Vector3 messagePos = default)
        {
            IsShowingTextEnd = false;
            textbox.SetActive(true);
            
            toastText.text = message;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            Canvas.ForceUpdateCanvases();
            
            rectTransform.position = messagePos;
            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            typingCoroutine = StartCoroutine(TypeText(message));
        }
        
        private IEnumerator TypeText(string message)
        {
            toastText.text = "";
            int i = 0;

            while (i < message.Length)
            {
                if (message[i] == '<') // 태그 시작 처리
                {
                    string tagStr = "";
                    while (i < message.Length && message[i] != '>')
                    {
                        tagStr += message[i];
                        i++;
                    }
                    if (i < message.Length) // 태그 끝 처리
                    {
                        tagStr += message[i];
                        i++;
                    }
                    toastText.text += tagStr;
                }
                else if (message[i] == '\\' && i + 1 < message.Length && message[i + 1] == 'n') // \n 처리
                {
                    toastText.text += '\n';
                    i += 2;
                }
                else
                {
                    toastText.text += message[i];
                    i++;
                    yield return new WaitForSeconds(typingSpeed);
                }
                if (IsShowingTextEnd) break;
            }
            
            // 플레이어 나갈 때까지 대기
            while (!IsShowingTextEnd)
            {
                yield return null;
            }
            
            textbox.SetActive(false);
            IsShowingTextEnd = false;
            typingCoroutine = null;
        }
    }
}