using UnityEngine;
using UnityEngine.UI;
namespace ToB.Scenes.Intro
{
    public class GamePlayUI:MonoBehaviour
    {
        [SerializeField] public GameObject playerInfoPanel;
        [SerializeField] public GameObject miniMapPanel;
        
        private void Awake()
        {
            UIManager.Instance.Init(this);
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(true);
        }
        
        //추후 HP바와 플레이어 체력 연동 필요.
    }
}