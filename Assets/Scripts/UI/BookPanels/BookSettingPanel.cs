using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB.UI.BookPanels
{
    public class BookSettingPanel : MonoBehaviour
    {
        public List<GameObject> pages;
        [SerializeField] private int lastPageIndex;

        private void Awake()
        {
            foreach (var page in pages)
            {
                page.SetActive(false);
            }

            lastPageIndex = -1;
        }

        private void OnEnable()
        {
            OpenLastPage();
        }

        private void OpenPage(int index)
        {
            if(lastPageIndex == index) return;
            
            foreach (var page in pages)
            {
                page.SetActive(false);
            }
            pages[index].SetActive(true);
            lastPageIndex = index;
        }

        private void OpenLastPage()
        {
            OpenPage(lastPageIndex);
        }
        
        public void OpenHelpPage()
        {
            UIManager.Instance.toastUI.Show("추후 추가 예정!");
            OpenPage(0);
        }
        
        public void OpenLoadPage()
        {
            UIManager.Instance.toastUI.Show("추후 추가 예정!");
            OpenPage(1);
        }

        public void OpenKeySettingPage()
        {
            OpenPage(2);
        }
        
        public void OpenAudioSettingPage()
        {
            OpenPage(3);
        }
        
        public void OpenQuitPage()
        {
            OpenPage(4);
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            //게임 종료
        }
    }
}
