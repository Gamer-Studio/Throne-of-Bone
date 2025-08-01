using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ToB.Scenes.Stage;
using UnityEngine.Events;

namespace ToB.UI
{
    public class EffectUI : MonoBehaviour
    {
            [Header( "Hit Effect" )]
            [SerializeField] public Image redOverlay;
            [SerializeField] public float hitFadeInTime = 0.1f;
            [SerializeField] public float hitFadeOutTime = 0.2f;
            [SerializeField] public float hitTargetAlpha = 0.5f;
            public UnityEvent onHitEffectEnd = new();
            
            [Header( "FadeOut Effect")]
            [SerializeField] public Image fadeOverlay;

            [SerializeField] public float fadeInTime;
            [SerializeField] public float fadeOutTime;
            [SerializeField] public float fadeTargetAlpha = 1f;
            
            private Tween hitTween;
            private Tween fadeTween;
            public void PlayHitEffect()
            {
                hitTween?.Kill();
                redOverlay.color = new Color(1,0,0,0);

                hitTween = redOverlay.DOFade(hitTargetAlpha, hitFadeInTime)
                    .OnComplete(() => redOverlay.DOFade(0, hitFadeOutTime)
                    .OnComplete(() => onHitEffectEnd.Invoke()));
            }

            private void IsFadeInEnded()
            {
                StageManager.Instance.player.isFadeOutEnded = true;
            }


            public void PlayFadeOutEffect()
            {
                fadeTween?.Kill();
                fadeOverlay.color = new Color(0, 0, 0, 0);
                fadeTween = fadeOverlay.DOFade(fadeTargetAlpha, fadeInTime)
                    .OnComplete(() =>
                    {
                        IsFadeInEnded();
                        fadeOverlay.DOFade(0, fadeOutTime);
                    });
            }
    }

}