using System.Collections.Generic;
using ToB.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ToB.UI
{
    public class RangeAtkCountUI : MonoBehaviour
    {
        private PlayerCharacter player;
        [SerializeField] List<Image> images = new List<Image>();
        private int maxRangeAttackCount = 0;
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Intro" && scene.name != "MainMenu")
            {
                gameObject.SetActive(true);
                player = PlayerCharacter.GetInstance();
                if (player != null)
                {
                    Init();
                }
                else
                {
                    Debug.Log("PlayerCharacter is null");
                }
            }
        }

        private void Init()
        {
            UpdateMaxCount(player.maxRangedAttack);
            player.OnRangedAttackStackChange.AddListener(UpdateCounter);
            //player.OnMaxRangedAttackChange.AddListener(UpdateMaxCount);
        }

        // 최대치 해방 시 최대치 늘려주고 카운터 갱신
        // 추후 PlayerCharacter에서 이벤트 하나 더 만들어 줘야 할 듯.
        
        public void UpdateMaxCount(int count)
        {
            maxRangeAttackCount = count;
            UpdateCounter(player.AvailableRangedAttack);
        }

        private void UpdateCounter(int curCount)
        {
            int value = curCount;
            for (int i = 0; i < 5; i++)
            {
                // 일단 SetActive를 true한 다음
                images[i].gameObject.SetActive(true);
                if (i < value)
                    images[i].color = new Color(0.8f, 0, 1, 1);
                else if (i < maxRangeAttackCount)
                    images[i].color = new Color(1, 1, 1, 0.5f);
                else
                    images[i].gameObject.SetActive(false);
            }
        }
    }
}