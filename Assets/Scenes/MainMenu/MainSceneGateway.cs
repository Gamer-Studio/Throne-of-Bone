using System;
using System.Collections;
using ToB.UI;
using UnityEngine;

namespace ToB
{
    public class MainSceneGateway : MonoBehaviour
    {
        IEnumerator Start()
        {
            UIManager.Instance.panelStack.Push(UIManager.Instance.introUI);
            yield return null;
        }

        private void OnDisable()
        {
            UIManager.Instance.panelStack.Clear();
        }
    }
}
