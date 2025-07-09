using System;
using ToB.UI;
using UnityEngine;

namespace ToB
{
    public class MainSceneGateway : MonoBehaviour
    {
        private void Awake()
        {
            
            UIManager.Instance.panelStack.Push(UIManager.Instance.introUI);
            Debug.Log("메인 씬 초기화 " + UIManager.Instance.panelStack.Count);
        }

        private void OnDestroy()
        {
            UIManager.Instance.panelStack.Clear();
        }
    }
}
