using System.Collections;
using ToB.Core;
using ToB.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using ToB.Entities.Skills;
using ToB.Scenes.Stage;

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
        [SerializeField] UIGaugeBar hpGaugeBar;
        [SerializeField] UIGaugeBar manaGaugeBar;
        [SerializeField] RangeAtkCountUI rangeAtkCountUI;
        
        private void Awake()
        {
            playerInfoPanel.SetActive(true);
            miniMapPanel.SetActive(false);
            saveIndicatorPanel.SetActive(false);
            ResourceManager.Instance.onGoldChanged.AddListener(UpdateGoldText);
            ResourceManager.Instance.onManaChanged.AddListener(UpdateManaText);
            ResourceManager.Instance.onMasterKeyChanged.AddListener(UpdateKeyText);
            
        }

        private void OnEnable()
        {
            InitText();
        }
        
        

        #region TestButton
        public void TestGoldAddButton()
        {
            ResourceManager.Instance.GiveGoldToPlayer(900000);
        }
        public void TestManaAddButton()
        {
            ResourceManager.Instance.GiveManaToPlayer(100);
        }
        public void TestPlayerInvincibleBtn()
        {
            StageManager.Instance.player.invincibility = true;
            UIManager.Instance.toastUI.Show("테스트용 플레이어 무적 활성화");
        }
        public void TestPlayerInvinOffBtn()
        {
            StageManager.Instance.player.invincibility = false;
            UIManager.Instance.toastUI.Show("테스트용 플레이어 무적 비활성화");
        }

        public void TestKeyAddButton()
        {
            ResourceManager.Instance.GiveMasterKeyToPlayer(1);
        }

        public void TestHPAddButton()
        {
            PlayerCharacter player = PlayerCharacter.Instance;
            player.stat.HealtoFullHp();
        }

        #endregion
        
        #region PlayerInfo
        private void InitText()
        {
            UpdateGoldText(ResourceManager.Instance.PlayerGold);
            UpdateManaText(ResourceManager.Instance.PlayerMana);
            UpdateKeyText(ResourceManager.Instance.MasterKey);
        }

        public void InitGages()
        {
            hpGaugeBar.Init();
            manaGaugeBar.Init();
            rangeAtkCountUI.Init();
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