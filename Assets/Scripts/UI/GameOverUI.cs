using ToB.Core;
using ToB.Player;
using ToB.Scenes.Intro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class GameOverUI : UIPanelBase
    {
        
        private PlayerCharacter player;


      
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
            if (scene.name != Defines.IntroScene && scene.name != Defines.MainMenuScene)
            {
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

        private void Init()
        {
            player.stat.onDeath.AddListener(GameOver);
        }
        
        private void GameOver()
        {
            gameObject.SetActive(true);
        }

        public void ReLoadScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }


        public override void Process(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public override void Cancel(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}