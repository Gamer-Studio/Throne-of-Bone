using System;
using System.Collections;
using System.Diagnostics;
using DG.Tweening;
using ToB.Core.InputManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;

namespace ToB.Entities
{
    public class Grimoire : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        SpriteRenderer sprite;
        Material material;
        
        [SerializeField] private bool isAppear;
        public bool IsAppear => isAppear;

        [SerializeField] private GameObject glowAnimation;
        [SerializeField] private VisualEffect glowVFX;
        [SerializeField] NPCSpeechBubble speechBubble;
        public NPCSpeechBubble SpeechBubble => speechBubble;
        
        Tween appearTween;

        private float motionTime = 0.3f;
        public float MotionTime => motionTime;
        
        private bool goNext;
        

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            material = GetComponent<SpriteRenderer>().material;
            material.SetFloat(Alpha,1);
        }

        IEnumerator Start()
        {
            if (!isAppear)
            {
                glowAnimation.SetActive(false);
                sprite.enabled = false;
                yield return null;
                glowVFX.SendEvent("OnStop");
            }
        }

        public void Appear()
        {
            material.SetColor(Color1, Color.white);
            sprite.enabled = true;
            glowAnimation.SetActive(true);
            speechBubble.SetText("");

            glowVFX.Play();
            
            appearTween?.Kill();
            appearTween = DOTween.To(() => material.GetColor(Color1), x =>  material.SetColor(Color1, x), Color.black, 0.3f);
        }

        public void Disappear()
        {
            material.SetColor(Color1, Color.black);
            appearTween?.Kill();
            
            appearTween = DOTween.To(() => material.GetColor(Color1),x =>material.SetColor(Color1, x), Color.white, 0.25f)
                .OnComplete(() =>
                {
                    sprite.enabled = false;

                    glowAnimation.SetActive(false);

                    glowVFX.Stop();
                });
        }

        public IEnumerator Say(string text)
        {
            yield return StartCoroutine(speechBubble.SetTextLetterByLetter(text));
            TOBInputManager.Instance.anyInteractionKeyAction += ProcessSaying;
            yield return new WaitUntil(() => goNext);
            TOBInputManager.Instance.anyInteractionKeyAction -= ProcessSaying;
            goNext = false;
        }

        public void ProcessSaying()
        {
            goNext = true;
        }
    }
}
