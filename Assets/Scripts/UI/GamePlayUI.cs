using System.Collections;
using ToB.Core;
using ToB.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace ToB.UI
{
    public class GamePlayUI:MonoBehaviour
    {
        [SerializeField] public GameObject miniMapPanel;
        [SerializeField] public GameObject saveIndicatorPanel;
        [SerializeField] public GameObject playerInfoPanel;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentGoldText;
        [SerializeField] public TMPro.TextMeshProUGUI CurrentManaText;
        [SerializeField] public GameObject KeyUI;
        [SerializeField] public TMPro.TextMeshProUGUI KeyText;
        [SerializeField] public Image saveIndicator;
        private void Awake()
        {
            UIManager.Instance.Init(this);
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(true);
            saveIndicatorPanel.SetActive(false);
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
        
        #region PlayerInfo
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
        #endregion
        
        #region SaveIndicator
        
        private Coroutine saveIndicatorCo;
        private Tween rotateTween;

        public void DuringSaveIndicator()
        {
            saveIndicatorPanel.SetActive(true);
            saveIndicator.color = new Color(saveIndicator.color.r,saveIndicator.color.g,saveIndicator.color.b, 1f);
            if (saveIndicatorCo != null)
            {
                StopCoroutine(saveIndicatorCo);
                saveIndicatorCo = null;
            }

            saveIndicatorCo = StartCoroutine(SaveIndicatorSpin());
        }

        private IEnumerator SaveIndicatorSpin()
        {
            // 기존 회전 Tween이 있다면 중단
            if (rotateTween != null && rotateTween.IsActive())
            {
                rotateTween.Kill();
                rotateTween = null;
            }

            // 무한 부드러운 회전 시작
            rotateTween = saveIndicator.transform
                .DOLocalRotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);

            /*
             while (true)
            {
                yield return null;
                if (SaveManager.Instance.isSaveEnded) break;
            }
            */
            if (rotateTween != null && rotateTween.IsActive())
            {
                rotateTween.Kill();
                rotateTween = null;
            }
            

            saveIndicator.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            saveIndicatorPanel.SetActive(false);
            saveIndicatorCo = null;
        }

        #endregion

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