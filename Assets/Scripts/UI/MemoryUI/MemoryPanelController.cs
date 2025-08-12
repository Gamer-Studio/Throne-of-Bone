using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI.MemoryUI
{
    public class MemoryPanelController : MonoBehaviour
    {
        [Header("스크롤 관련")]
        private ScrollRect scrollRect;
        
        [Header("일지 패널 스위칭 관련")]
        [SerializeField] private Button MonsterPanelSwitchButton;
        [SerializeField] private Button MemoryPanelSwitchButton;
        [SerializeField] private Image MonsterPanelBtnImg;
        [SerializeField] private Image MemoryPanelBtnImg;
        [SerializeField] private GameObject MonsterPanel;
        [SerializeField] private GameObject MemoryPanel;
        
        [Header("선택된 패널 관리")]
        private GameObject selectedPanel;
        [SerializeField] private TextMeshProUGUI RightPageTitleText;
        [SerializeField] private Image SelectedMemoeyImage;
        [SerializeField] private TextMeshProUGUI MemoryDescriptionText;
        

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            MonsterPanelSwitchButton.onClick.AddListener(() => PanelSelect(MonsterPanel, MonsterPanelBtnImg));
            MemoryPanelSwitchButton.onClick.AddListener(() => PanelSelect(MemoryPanel, MemoryPanelBtnImg));
        }

        private void OnEnable()
        {
            // 기본 선택값은 몬스터 패널
            PanelSelect(MonsterPanel, MonsterPanelBtnImg);
        }

        
        private void PanelSelect(GameObject _selectedPanel, Image _selectedBtnImg)
        {
            VariableReset();
            
            selectedPanel = _selectedPanel;
            _selectedBtnImg.color = Color.yellow;
            selectedPanel.SetActive(true);
            ScrollRectSetting();
        }
        private void VariableReset()
        {
            MonsterPanel.SetActive(false);
            MemoryPanel.SetActive(false);
            MonsterPanelBtnImg.color = Color.gray;
            MemoryPanelBtnImg.color = Color.gray;
            RightPageTitleText.text = "???";
            SelectedMemoeyImage.sprite = null;
            SelectedMemoeyImage.color = new Color(1, 1, 1, 0);
            MemoryDescriptionText.text = "???";
        }
        private void ScrollRectSetting()
        {
            scrollRect.viewport = selectedPanel.GetComponent<RectTransform>();
            scrollRect.content = selectedPanel.gameObject.transform.GetChild(0).GetComponent<RectTransform>();
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}
