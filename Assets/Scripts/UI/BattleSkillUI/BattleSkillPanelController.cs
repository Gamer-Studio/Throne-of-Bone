using ToB.Entities.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI
{
    public class BattleSkillPanelController : MonoBehaviour
    {
        [Header("스크롤 관련")]
        private ScrollRect scrollRect;
        
        [Header("스킬 패널 스위칭 관련")]        
        [SerializeField] private Button OFN_PanelSwitchButton;
        [SerializeField] private Button DEF_PanelSwitchButton;
        [SerializeField] private Button SUP_PanelSwitchButton;
        [SerializeField] private GameObject OFN_SkillPanel;
        [SerializeField] private GameObject DEF_SkillPanel;
        [SerializeField] private GameObject SUP_SkillPanel;
        [SerializeField] private TMPro.TextMeshProUGUI SkillTypeText;
        private GameObject selectedPanel;
        private SkillType selectedSkillType;
        
        [Header("선택된 스킬 관련")]
        [SerializeField] private TMPro.TextMeshProUGUI SkillNameText;
        [SerializeField] private TMPro.TextMeshProUGUI SkillDescriptionText;
        [SerializeField] private TMPro.TextMeshProUGUI SkillCostText;
        
        [SerializeField] public Sprite Unacquired;
        [SerializeField] public Sprite Acquired;
        [SerializeField] public Sprite Deactivated;
        [SerializeField] public Sprite LockedByTier;
        
        private int selectedSkillID;
        private Button selectedButton;
        
        
        [Header("나머지 버튼들")]
        [SerializeField] private Button LearnButton;
        [SerializeField] private Button ExitButton;


        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            OFN_PanelSwitchButton.onClick.AddListener(() => SkillPanelSwitch(SkillType.OFN));
            DEF_PanelSwitchButton.onClick.AddListener(() => SkillPanelSwitch(SkillType.DEF));
            SUP_PanelSwitchButton.onClick.AddListener(() => SkillPanelSwitch(SkillType.SUP));
            LearnButton.onClick.AddListener(OnLearnButtonClick);
            ExitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void ScrollRectSetting()
        {
            scrollRect.viewport = selectedPanel.GetComponent<RectTransform>();
            scrollRect.content = selectedPanel.gameObject.transform.GetChild(0).GetComponent<RectTransform>();
        }
        private void OnEnable()
        {
            VariableReset();
            SkillPanelSwitch(selectedSkillType);
        }

        /// <summary>
        /// 내부 변수 및 텍스트를 리셋해 줍니다.
        /// </summary>
        private void VariableReset(SkillType skillType = SkillType.OFN)
        {
            selectedSkillType = skillType;
            selectedSkillID = 0;
            selectedButton = null;
            SkillNameText.text = "";
            SkillDescriptionText.text = "선택된 스킬 없음";
            SkillCostText.text = "";
        }
        
        /// <summary>
        /// 버튼이 클릭되면 호출될 메서드.
        /// </summary>
        /// <param name="skillID"></param>
        /// <param name="button"></param>
        public void OnSkillButtonClick(int skillID, GameObject button)
        {
            // 앞서 누른 버튼과 다른 버튼을 누르면
            if (selectedSkillID != skillID)
            {
                selectedSkillID = skillID;
                if (selectedButton != null) selectedButton.GetComponent<EverySkillButtons>().UnClicked();
                selectedButton = button.GetComponent<Button>();
                SelectedSkillTextUpdate();
            }
        }

        private void SelectedSkillTextUpdate()
        {
            SkillNameText.text = BattleSkillManager.Instance.skillDB.GetSkillById(selectedSkillID).skillName;
            SkillDescriptionText.text = "스킬설명";
            //SkillDescriptionText.text = BattleSkillManager.Instance.skillDB.GetSkillById(selectedSkillID).skillDescription;
            switch (BattleSkillManager.Instance.GetSkillState(selectedSkillID))
            {
                case SkillState.Acquired:
                    SkillCostText.text = "이미 배운 스킬입니다.";
                    break;
                case SkillState.Unacquired:
                    SkillCostText.text =
                        $"필요 자원 : {BattleSkillManager.Instance.skillDB.GetSkillById(selectedSkillID).goldCost} G" +
                        $" + {BattleSkillManager.Instance.skillDB.GetSkillById(selectedSkillID).manaCost} M";
                    break;
                case SkillState.Deactivated:
                    SkillCostText.text =
                        "필요 자원 : 0 G" +
                        $" + {BattleSkillManager.Instance.skillDB.GetSkillById(selectedSkillID).manaCost} M";
                    break;
            }
        }
        
        /// <summary>
        /// 선택된 스킬 종류에 따라 패널을 활성화합니다.
        /// </summary>
        /// <param name="skillType"></param>
        private void SkillPanelSwitch(SkillType skillType)
        {
            selectedSkillType = skillType;
            switch (skillType)
            {
                case SkillType.OFN:
                    OFN_SkillPanel.SetActive(true);
                    DEF_SkillPanel.SetActive(false);
                    SUP_SkillPanel.SetActive(false);
                    selectedPanel = OFN_SkillPanel;
                    SkillTypeText.text = "파괴의 기억";
                    break;
                case SkillType.DEF:
                    OFN_SkillPanel.SetActive(false);
                    DEF_SkillPanel.SetActive(true);
                    SUP_SkillPanel.SetActive(false);
                    selectedPanel = DEF_SkillPanel;   
                    SkillTypeText.text = "수호의 기억";
                    break;
                case SkillType.SUP:
                    OFN_SkillPanel.SetActive(false);
                    DEF_SkillPanel.SetActive(false);
                    SUP_SkillPanel.SetActive(true);
                    selectedPanel = SUP_SkillPanel;
                    SkillTypeText.text = "전장의 기억";
                    break;
            }
            ScrollRectSetting();
            VariableReset(skillType);
        }
        
        #region Button Event
        //Comment : 각 버튼들에 스크립트를 넣을 수도 있었는데, 그러기엔 스크립트가 너무 많아질 것 같아
        // 아래 버튼 이밴트들은 인스펙터 할당으로 하려고 합니다(역할이 변할 일 없는 버튼들)
        
        /// <summary>
        /// 선택한 스킬을 배우기 누르면 벌어지는 이벤트.
        /// 1. 스킬 배우기를 시도합니다.
        /// 2. 그 결과에 따라 선택된 버튼의 버튼 이미지를 갱신합니다.
        /// </summary>
        public void OnLearnButtonClick()
        {
            bool isSucceed = BattleSkillManager.Instance.LearnSkill(selectedSkillID);
            if (!isSucceed) return;
            // 실패 시 갱신 안하고 성공 시 정보 갱신
            selectedButton.GetComponent<EverySkillButtons>().UpdateButtonImage();
            SelectedSkillTextUpdate();
        }
        
        public void OnExitButtonClick()
        {
            UIManager.Instance.mainBookUI.CloseBook();
        }
       
        #endregion
       
        
        
    }
}
