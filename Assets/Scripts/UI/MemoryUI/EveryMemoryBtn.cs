using TMPro;
using ToB.Memories;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI.MemoryUI
{
    public class EveryMemoryBtn : MonoBehaviour
    {
        private Button button;
        [SerializeField] private TextMeshProUGUI ButtonNameText;
        [SerializeField] private TextMeshProUGUI MemoryDescriptionText;
        [SerializeField] private TextMeshProUGUI RightPageTitleText;
        [SerializeField] private Image SelectedImage;
        [SerializeField] private int buttonID;
        
        private void Awake()
        {
            if (!button) button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            SetText();
        }

        private void SetText()
        {
            if (MemoriesManager.Instance.MemoriesStates[buttonID])
            {
                if (buttonID.ToString().Length == 5)
                    ButtonNameText.text = "- " + MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).name;
                else if (buttonID.ToString().Length == 4)
                {
                    ButtonNameText.text =
                        MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).name;
                }
            }
            else
            {
                if (buttonID.ToString().Length == 5)
                    ButtonNameText.text = "- ???";
                else if (buttonID.ToString().Length == 4)
                {
                    ButtonNameText.text = "#" + (buttonID + 1).ToString()[3] + ". ???";
                }

            }
        }
        private void SetRightPageInfo()
        {
            if (MemoriesManager.Instance.MemoriesStates[buttonID])
            {
                RightPageTitleText.text = MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).name;
                MemoryDescriptionText.text = MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).description;
                SelectedImage.sprite = MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).relatedIcon;
                SelectedImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                RightPageTitleText.text = "???";
                MemoryDescriptionText.text = "???";
                SelectedImage.sprite = null;
                SelectedImage.color = new Color(1, 1, 1, 0);
            }
        }
        private void OnClick()
        {
            SetRightPageInfo();
        }
        

    }
}
