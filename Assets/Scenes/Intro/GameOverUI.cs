using ToB.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ToB.Scenes.Intro
{
    public class GameOverUI : MonoBehaviour
    {
        
        private PlayerCharacter player;


      
        private void Awake()
        {
            UIManager.Instance.Init(this);
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
                player = PlayerCharacter.GetInstance();
                if (player != null)
                {
                    Init();
                }
                else
                {
                    Debug.Log("PlayerCharacter is null");
                }
                Debug.Log("GameOver Scene Ready");
            }
        }

        private void Init()
        {
            player.stat.onHpChanged.AddListener(GameOver);
        }
        
        private void GameOver(float curHp)
        {
            if (curHp <= 0)
            {
                gameObject.SetActive(true);
            }
        }

        public void ReLoadScene()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(sceneName);
        }


    }
}