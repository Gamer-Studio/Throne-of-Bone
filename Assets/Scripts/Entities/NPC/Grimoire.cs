using System;
using System.Collections;
using DG.Tweening;
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
        
        Tween appearTween;

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

        public void OnAppear()
        {
            material.SetColor(Color1, Color.white);
            sprite.enabled = true;
            glowAnimation.SetActive(true);

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
    }
}
