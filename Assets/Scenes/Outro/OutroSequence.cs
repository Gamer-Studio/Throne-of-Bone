using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToB.Scenes
{
    public class OutroSequence : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        IEnumerator Start()
        {
            text.alpha = 0;
            yield return null;
            text.DOFade(1, 2);
            yield return new WaitForSeconds(5);
            text.DOFade(0, 2);

            yield return new WaitForSeconds(3);

            SceneManager.LoadScene("MainMenu");
        }
    }
}
