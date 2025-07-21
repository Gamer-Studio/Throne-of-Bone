using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI
{
    public class ToastUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform rectTransform;


        [Header("Tween Values")]
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float displayDuration;
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private Vector3 startPosOffset;
        [SerializeField] private Vector3 endPosOffset;
        
        private Tween toastTween;
        private Vector3 basePos;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponentInChildren<Image>();
            image.color = Color.black;
            this.gameObject.SetActive(false);
            basePos = rectTransform.localPosition;
        }

        public void Show(string message)
        {
            gameObject.SetActive(true);
            
            text.text = message;
            // 왠진 몰라도... 이걸 앞뒤로 업뎃해줘야 잘 된다네요. 의도된 반복입니다!
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            Canvas.ForceUpdateCanvases();
            
            image.color = Color.clear;
            rectTransform.localPosition = basePos + startPosOffset;

            // 이전 트윈이 있으면 지우기
            toastTween?.Kill();

            // 올라오면서 알파 올리기
            toastTween = DOTween.Sequence()
                .Append(image.DOFade(1, fadeInDuration))
                .Join(rectTransform.DOLocalMove(basePos, fadeInDuration))
                .AppendInterval(displayDuration)
                // 내려가면서 알파 내리기
                .Append(image.DOFade(0, fadeOutDuration))
                .Join(rectTransform.DOLocalMove(basePos + endPosOffset, fadeOutDuration))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }
        private void OnDisable()
        {
            toastTween?.Kill();
        }
    }
}
