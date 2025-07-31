using System.Collections;
using UnityEngine;

namespace ToB.Utils
{
    public static class GraphicUtil
    {
        public static void DamageColorOverlay(MonoBehaviour owner, SpriteRenderer sprite)
        {
            owner.StartCoroutine(DamageColorOverlayCoroutine(sprite));
        }
        static IEnumerator DamageColorOverlayCoroutine(SpriteRenderer sprite)
        {
            sprite.material.SetFloat("_Alpha", 1);
            float duration = 0.3f;
            float remainedTime = duration;

            while (remainedTime > 0)
            {
                yield return null;
                remainedTime -= Time.deltaTime;
                if (remainedTime < 0) remainedTime = 0;
                sprite.material.SetFloat("_Alpha", remainedTime / duration);
            }
        }
    }
}