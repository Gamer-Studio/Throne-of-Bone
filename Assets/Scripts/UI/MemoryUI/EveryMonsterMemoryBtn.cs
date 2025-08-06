using TMPro;
using ToB.Memories;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.UI.MemoryUI
{
    public class EveryMonsterMemoryBtn : MonoBehaviour
    {
        private Button button;
        public bool isSelected;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI MonsterNameText;
        [SerializeField] private TextMeshProUGUI MonsterDescriptionText;
        [SerializeField] private int buttonID;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            SetText();
        }

        private void SetText()
        {
            if (MemoriesManager.Instance.MemoriesStates[buttonID])
            {
                MonsterNameText.text = MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).name;
                MonsterDescriptionText.text = MemoriesManager.Instance.memoriesDB.GetMemoriesById(buttonID).description;
            }
            else
            {
                MonsterNameText.text = "???";
                MonsterDescriptionText.text = "???";
            }
        }

        private void OnClick()
        {
            SetText();
        }
        

    }
}
