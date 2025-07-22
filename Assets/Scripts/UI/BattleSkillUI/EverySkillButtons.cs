using ToB.Entities.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI
{
    public class EverySkillButtons : MonoBehaviour
    {
        private Button button;
        public bool isSelected;
        [SerializeField] public GameObject Selector;
        [SerializeField] private BattleSkillPanelController panelController;
        [SerializeField] public Image buttonImage;
        
        [SerializeField] public int Skill_ID;
        
        /// <summary>
        /// 버튼이 처음 Awake될 때 생성됩니다.
        /// 이거를 버튼 프리팹으로 만들어서 미리 할당해 두는 게 나을지 고민되긴 합니다.
        /// 흠... 좀 무거운가?
        /// </summary>
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
            // 실수로 스킬 ID 설정 안 한 경우 버튼 오브젝트 끄기
            if (Skill_ID == 0)
            {
                button.image.enabled = false;
                button.interactable = false;
            }
            //Selector = transform.Find("SelectedSprite").gameObject;
            //buttonImage = GetComponentInChildren<Image>();
            //panelController = GetComponentInParent<BattleSkillPanelController>();
        }

        /// <summary>
        /// 버튼이 켜질 때마다 활성화됩니다.
        /// 1. 버튼의 선택 정보를 초기화합니다.
        /// 2. 스킬의 상태에 따라 이미지를 갱신합니다.
        /// </summary>
        private void OnEnable()
        {
            isSelected = false;
            Selector.SetActive(isSelected);
            if (BattleSkillManager.Instance != null && Skill_ID != 0)
                UpdateButtonImage();
        }
        
        /// <summary>
        /// 버튼이 클릭되면 실행되는 메서드입니다.
        /// 1. Selector가 활성화됩니다.
        /// 2. 스킬패널컨트롤러에 이 버튼의 정보를 보냅니다. (패널 컨트롤러는 해당 정보를 캐싱합니다)
        /// 3. 
        /// </summary>
        private void OnClicked()
        {
            isSelected = true;
            Selector.SetActive(true);
            panelController.OnSkillButtonClick(Skill_ID, this.gameObject);
        }

        public void UnClicked()
        {
            isSelected = false;
            Selector.SetActive(isSelected);
        }

        /// <summary>
        /// 스킬의 상태에 따라 버튼의 이미지를 갱신합니다.
        /// </summary>
        public void UpdateButtonImage()
        {
            button.interactable = true;
            switch (BattleSkillManager.Instance.PlayerSkillStates[Skill_ID])
            {
                case SkillState.Unacquired:
                    buttonImage.sprite = panelController.Unacquired;
                    break;
                case SkillState.Acquired:
                    buttonImage.sprite = panelController.Acquired;
                    break;
                case SkillState.Deactivated:
                    buttonImage.sprite = panelController.Deactivated;
                    break;
            }
            /* 티어가 생기면 주석 해제
            if (StageManager.Instance.tier > BattleSkillManager.Instance.skillDB.GetSkillById(Skill_ID).tier)
                {
                    buttonImage.sprite = panelController.LockedByTier;
                    button.interactable = false;
                }
            */
        }

       
        
        
    }
}
