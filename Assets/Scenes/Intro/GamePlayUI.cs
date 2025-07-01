using ToB.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   
namespace ToB.Scenes.Intro
{
    public class GamePlayUI:MonoBehaviour
    {
        [SerializeField] public GameObject playerInfoPanel;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentGoldText;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentManaText;
        [SerializeField] public GameObject miniMapPanel;
        
        private void Awake()
        {
            UIManager.Instance.Init(this);
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(true);
            ResourceManager.Instance.onGoldChanged.AddListener(UpdateGoldText);
            ResourceManager.Instance.onManaChanged.AddListener(UpdateManaText);
            InitText();
        }
        #region TestButton
        public void TestGoldAddButton()
        {
            ResourceManager.Instance.GiveGoldToPlayer(10);
        }
        public void TestManaAddButton()
        {
            ResourceManager.Instance.GiveManaToPlayer(10);
        }
        public void TestGoldSubButton()
        {
            ResourceManager.Instance.IsPlayerHaveEnoughResources(50,0);
        }
        public void TestManaSubButton()
        {
            ResourceManager.Instance.IsPlayerHaveEnoughResources(10,50);       
        }
        #endregion

        private void InitText()
        {
            UpdateGoldText(ResourceManager.Instance.PlayerGold);
            UpdateManaText(ResourceManager.Instance.PlayerMana);       
        }

        private void UpdateGoldText(int gold)
        {
            CurrentGoldText.text = $"{gold.ToString()} G";
        }

        private void UpdateManaText(int mana)
        {
            CurrentManaText.text = $"{mana.ToString()} M";
        }

        public void ClearActiveUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("ClearActiveUI 실행됨");
                if (UIManager.Instance.IsThereOverlayUI())
                {
                    Debug.Log("활성화된 UI 존재함. UI 닫음.");
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.mainBookUI.gameObject.SetActive(false);
                }
            }
        }
    }
}