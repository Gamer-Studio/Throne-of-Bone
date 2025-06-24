using UnityEngine;
using UnityEngine.UI;

namespace ToB.Scenes.Intro
{
    public class MainBookUI:MonoBehaviour
    {
        
        [SerializeField] public GameObject[] panels;
        private GameObject currentPanel;
        [SerializeField] public Button[] buttons;
       
        private void Awake()
        {
           UIManager.Instance.Init(this);
           InitButtons();
           InitPanels();
        }

        private void InitButtons()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => ShowPanel(index));
                // 이벤트 리스너 각각 등록
            }
        }
        private void InitPanels()
        {
            if (currentPanel == null)
            {
                panels[0].SetActive(true);
            }
            else
            {
                currentPanel.SetActive(true);
            }
        }
        
        private void ShowPanel(int indexToShow)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].SetActive(i == indexToShow);
                // true인 것만 SetActive, 나머진 false
            }
        }




    }
}