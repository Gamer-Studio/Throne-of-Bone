using ToB.Core;
using ToB.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.UI
{
    public class GamePlayUI:MonoBehaviour
    {
        [SerializeField] public GameObject playerInfoPanel;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentGoldText;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentManaText;
        [SerializeField] public GameObject miniMapPanel;
        [SerializeField] public GameObject KeyUI;
        [SerializeField] public TMPro.TextMeshProUGUI KeyText;
        private void Awake()
        {
            UIManager.Instance.Init(this);
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(true);
            ResourceManager.Instance.onGoldChanged.AddListener(UpdateGoldText);
            ResourceManager.Instance.onManaChanged.AddListener(UpdateManaText);
            ResourceManager.Instance.onMasterKeyChanged.AddListener(UpdateKeyText);
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

        public void TestKeyAddButton()
        {
            ResourceManager.Instance.GiveMasterKeyToPlayer(1);
        }

        public void TestHPAddButton()
        {
            PlayerCharacter player = PlayerCharacter.GetInstance();
            player.stat.Hp = player.stat.maxHp;
        }

        #endregion

        private void InitText()
        {
            UpdateGoldText(ResourceManager.Instance.PlayerGold);
            UpdateManaText(ResourceManager.Instance.PlayerMana);
            UpdateKeyText(ResourceManager.Instance.MasterKey);
        }

        private void UpdateGoldText(int gold)
        {
            CurrentGoldText.text = $"{gold.ToString()} G";
        }

        private void UpdateManaText(int mana)
        {
            CurrentManaText.text = $"{mana.ToString()} M";
        }
        
        private void UpdateKeyText(int keyAmount)
        {
            KeyText.text = $"{keyAmount.ToString()}";
            if (keyAmount > 0)
            {
                KeyUI.SetActive(true);
            }
            else
            {
                KeyUI.SetActive(false);
            }
        }

        public void ClearActiveUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (UIManager.Instance.IsThereOverlayUI())
                {
                    UIManager.Instance.wideMapUI.gameObject.SetActive(false);
                    UIManager.Instance.mainBookUI.gameObject.SetActive(false);
                }
            }
        }
    }
}