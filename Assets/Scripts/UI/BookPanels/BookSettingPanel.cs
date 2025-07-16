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
            OpenPage(0);
        }
        
        public void OpenLoadPage()
        {
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
        }
    }
}
