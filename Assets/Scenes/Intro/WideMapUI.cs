using UnityEngine;

namespace ToB.Scenes.Intro
{
    public class WideMapUI:MonoBehaviour
    {
        [SerializeField] private GameObject wideMapPanel;
        
        private void Awake()
        {
            UIManager.Instance.Init(this);
            wideMapPanel.SetActive(false);
        }
    }
}