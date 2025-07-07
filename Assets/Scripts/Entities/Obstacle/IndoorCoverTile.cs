using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

namespace ToB.Entities.Obstacle
{
    public class IndoorCoverTile : MonoBehaviour
    {
        private Tilemap _Tilemap;
        [SerializeField] public float FadeTime;
        private Coroutine C_Fadein;
        private Coroutine C_Fadeout;
        
        private void Awake()
        {
            _Tilemap = GetComponent<Tilemap>();
        }

        private void FadeIn()
        {
            if (C_Fadeout != null)
            {
                StopCoroutine(C_Fadeout);
                C_Fadeout = null;
            }

            if (C_Fadein == null)
                C_Fadein = StartCoroutine(FadeinCoroutine());
        }

        private IEnumerator FadeinCoroutine()
        {
            float r = _Tilemap.color.r;
            float g = _Tilemap.color.g;
            float b = _Tilemap.color.b;
            float alpha = _Tilemap.color.a;
            while (_Tilemap.color.a < 1 || C_Fadein == null)
            {
                alpha += Time.deltaTime / FadeTime;
                _Tilemap.color = new Color(r,g,b,alpha);
                yield return null;
            }
            C_Fadein = null;
        }

        private IEnumerator FadeoutCoroutine()
        {
            float r = _Tilemap.color.r;
            float g = _Tilemap.color.g;
            float b = _Tilemap.color.b;
            float alpha = _Tilemap.color.a;
            while (_Tilemap.color.a > 0 || C_Fadeout == null)
            {
                alpha -= Time.deltaTime / FadeTime;
                _Tilemap.color = new Color(r,g,b,alpha);
                yield return null;
            }
            C_Fadeout = null;
        }

        private void FadeOut()
        {
            if (C_Fadein != null)
            {
                StopCoroutine(C_Fadein);
                C_Fadein = null;
            }
            if (C_Fadeout == null)
                C_Fadeout = StartCoroutine(FadeoutCoroutine());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                FadeOut();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                FadeIn();
            }
        }


    }
}