using System;
using System.Collections;
using TMPro;
using ToB.UI;
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
            for (int i = 0; i < message.Length; i++)
            {
                if (IsShowingTextEnd) break;
                toastText.text = message.Substring(0, i + 1);
                yield return new WaitForSeconds(typingSpeed);
            }
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