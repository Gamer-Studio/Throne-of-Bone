using System.Collections.Generic;
using ToB.Core;
using ToB.Entities.Skills;
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
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            //SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != Defines.IntroScene && scene.name != Defines.MainMenuScene)
            {
                gameObject.SetActive(true);
                player = PlayerCharacter.Instance;
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

        public void Init()
        {
            gameObject.SetActive(true);
            
            player = PlayerCharacter.Instance;
            UpdateMaxCount(BattleSkillManager.Instance.BSStats.RangeAtkStack);
            player.OnRangedAttackStackChange.AddListener(UpdateCounter);
            BattleSkillManager.Instance.onRangeAtkStackChanged.AddListener(UpdateMaxCount);
        }
      
        public void UpdateMaxCount(int count)
        {
            maxRangeAttackCount = count + player.maxRangedAttack;
            UpdateCounter(player.AvailableRangedAttack);
        }

        private void UpdateCounter(int curCount)
        {
            Debug.Log("Count");
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