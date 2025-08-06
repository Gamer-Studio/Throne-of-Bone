using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Utils.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.CutScene
{
    public class StageIntroSequence : MonoBehaviour
    {
        public TextMeshProUGUI centerText;
        public TextMeshProUGUI skipText;

        private Coroutine textCoroutine;
        private Sequence seq;

        private bool skipCall;

        public GameObject glowObj;
        public SpriteRenderer glowSprite;
        
        private Tween glowFadeTween;
        private Tween glowScaleTween;
        private void Awake()
        {
            skipCall = false;
            TOBInputManager.Instance.anyInteractionKeyAction += Skip;
        }
        IEnumerator Start()
        {
            centerText.alpha = 0;
            glowObj.transform.localScale = Vector3.zero;
            glowSprite.color.WithAlpha(0);
            
            yield return null;
            yield return StartCoroutine(ShowText("..."));
            skipText.DOFade(0, 1f);
            yield return StartCoroutine(ShowText("..."));
            yield return StartCoroutine(ShowText("...들리십니까?",1.7f));
            yield return StartCoroutine(ShowText("제 목소리가... 그대에게 닿기를.",  1.7f, stopDuration:1.5f));
            yield return new WaitForSeconds(0.2f);
            
            yield return StartCoroutine(ShowText("이곳은 너무도 조용하고…", 1.9f, stopDuration:1.4f));
            yield return StartCoroutine(ShowText("너무나도 추운 곳입니다.", 1.9f, stopDuration:1.4f));
            yield return new WaitForSeconds(0.2f);
            
            glowFadeTween = glowSprite.DOFade(0.3f, 0.5f);
            glowScaleTween = glowObj.transform.DOScale(50f, 0.5f);
            yield return StartCoroutine(ShowText("하지만, 그대라면…", stopDuration:1.4f));
            yield return StartCoroutine(ShowText("이번에는… 다를지도 모르지요.", stopDuration:1.6f));
            
            glowFadeTween?.Kill();
            glowScaleTween?.Kill();
            glowSprite.DOFade(0.7f, 0.5f);
            glowObj.transform.DOScale(100, 0.5f);
            yield return StartCoroutine(ShowText("이제 … 일어나 주세요.", stopDuration:1.8f));
            
            glowFadeTween?.Kill();
            glowScaleTween?.Kill();
            glowSprite.DOFade(1f, 0.5f);
            glowObj.transform.DOScale(8000, 10f);
            centerText.color = Color.black;
            yield return StartCoroutine(ShowText("그대는… 마지막 희망이니까요.",2f, 2.5f));
            TOBInputManager.Instance.anyInteractionKeyAction -= Skip;
            SceneManager.LoadScene(Defines.StageScene);
        }

        
        IEnumerator ShowText(string text, float fadeDuration = 1.4f, float stopDuration = 1.2f)
        {
            if (seq != null && seq.IsActive())
            {
                seq.Kill();
            }
            
            seq = DOTween.Sequence();
            
            centerText.text = text;
            seq.Append(centerText.DOFade(1, fadeDuration));
            seq.AppendInterval(stopDuration);
            seq.Append(centerText.DOFade(0, fadeDuration));
            seq.AppendInterval(0.5f);

            while (seq.IsActive() && seq.IsPlaying())
            {
                if (skipCall)
                {
                    seq.Kill();
                    centerText.alpha = 0;   
                    skipCall = false;
                    yield break;
                }

                yield return null;
            }
          
        }
        
        void Skip()
        {
            skipCall = true;
        }

        private void OnDestroy()
        {
            TOBInputManager.Instance.anyInteractionKeyAction -= Skip;
            glowFadeTween?.Kill();
            glowScaleTween?.Kill();
            skipText.DOKill();
        }
    }
}
