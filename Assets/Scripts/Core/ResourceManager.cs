using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ToB.Core
{
    
    
    public class ResourceManager : DDOLSingleton<ResourceManager>, IJsonSerializable
    {
        // 싱글톤 선언 : Singleton<ResourceManager>.Instance.~~로 필요 시 호출
        private int playerGold;
        private int playerMana;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // SaveManager가 Load를 끝내기를 기다리는 코루틴 혹은 async 필요? 보니까 알아서 해주는 것 같은데
            // 애초에 LoadJson을 호출해 줄 필요가 없어 보이긴 하지만... 상황에 따라 Init이 필요 없을지도. 거의 외부참조용 매니저라서
            // Init();
        }

        private void Init()
        {
            //if (세이브파일에서 시작한 게 아니면 0원 넣기: 세이브/로드 방식을 보고 조건 거는 방식을 결정하여 Init할 예정)
            PlayerGold = 0;
            PlayerMana = 0;
        }
        public int PlayerGold
        { 
            get => playerGold;
            set
            {
                playerGold = value;
                onGoldChanged?.Invoke(playerGold);
            }
        }
        public int UsedGold { get; set; }

        public int PlayerMana
        { 
            get => playerMana;
            set
            {
                playerMana = value;
                onManaChanged?.Invoke(playerMana);
            }
        }
        public int UsedMana { get; set; }

        public UnityEvent<int> onGoldChanged = new();
        public UnityEvent<int> onManaChanged = new();
        /// <summary>
        /// 플레이어에게 골드를 줍니다.
        /// </summary>
        /// <param name="gold"></param>
        public void GiveGoldToPlayer(int gold)
        {
            PlayerGold += gold;
        }
        
        /// <summary>
        /// 플레이어에게 마나결정을 줍니다.
        /// </summary>
        /// <param name="mana"></param>
        public void GiveManaToPlayer(int mana)
        {
            PlayerMana += mana;
        }
        
        /// <summary>
        /// 플레이어가 충분한 자원을 소지했는지 물어봅니다. 골드와 마력 모두 체크합니다. 하나라도 모자라면 false를 return하고
        /// 무슨 자원이 부족한지를 로그 띄웁니다.
        /// 둘 다 충분하다면 각 자원을 소비합니다.
        /// </summary>
        /// <param name="requiredGold"></param>
        /// <param name="requiredMana"></param>
        /// <returns></returns>
        public bool IsPlayerHaveEnoughResources(int requiredGold, int requiredMana)
        {
            bool _isPlayerHaveEnoughResources = true;
            
            if (PlayerGold < requiredGold)
            {
                Debug.Log("Not enough gold");
                _isPlayerHaveEnoughResources = false;
            }
            if (PlayerMana < requiredMana)
            {
                Debug.Log("Not enough mana");
                _isPlayerHaveEnoughResources = false;
            }

            if (_isPlayerHaveEnoughResources)
            {
                UseGold(requiredGold);
                UseMana(requiredMana);
            }

            return _isPlayerHaveEnoughResources;
        }

        /// <summary>
        /// 플레이어가 골드 사용 시 호출
        /// </summary>
        /// <param name="requiredGold"></param>
        public void UseGold(int requiredGold)
        {
            PlayerGold -= requiredGold;
            UsedGold += requiredGold;
            Debug.Log($"{requiredGold} 골드를 사용했습니다.");
        }
        
        /// <summary>
        /// 플레이어가 마력 결정 사용 시 호출
        /// </summary>
        /// <param name="requiredMana"></param>
        public void UseMana(int requiredMana)
        {
            PlayerMana -= requiredMana;
            UsedMana += requiredMana;
            Debug.Log($"{requiredMana} 마나결정을 사용했습니다");
        }
        #region DeathPenelty
        /* 시체 오브젝트 클래스 만들어지면 그때 주석 해제 (Corpse)
         
        /// <summary>
        /// 시체에게 자원 소지량을 전달해주는 메서드.
        /// 호출 시 ResourceManager.Instance.GiveResourcesToCorpse(this); 이렇게 하는 걸 생각했습니다.
        /// out으로 전달해줄까도 생각해 봤는데 굳이 그렇게까지 책임을 나눌 필요는 없을 것 같아서 메서드 하나로 일괄처리하는 느낌?
        /// 아직 따듯한 시체인지 구분은 시체 저장 시의 index를 활용하는 게 어떨까 싶어요.
        /// 굳이 따듯한 시체 위치를 특정 매니저가 캐싱하는 것도 웃긴 것 같기도 하구요. 해봤자 StageManager?
        /// 럽샷하는 경우(플레이어가 먼저 죽고 날아간 검기에 몹이 죽는다던지)의 골드/마나결정은
        /// 획득되게 할 건지, 획득하면 어떻게 처리할 것인지 같은 타이밍 이슈는 추후 생각하는 거로 하겠습니다.
        /// </summary>
        public void GiveResourcesToCorpse(Corpse corpse)
        {
            corpse.Gold = DropAllGoldsToCorpse();
            corpse.Mana = DropAllManaToCorpse();
        }
        
        */
        public int DropAllGoldsToCorpse()
        {
            int totalGold = PlayerGold;
            PlayerGold = 0;
            return totalGold;
        }
        public int DropAllManaToCorpse()
        {
            int totalMana = PlayerMana + UsedMana;
            PlayerMana = 0;
            return totalMana;
        }
        
        #endregion
        
        #region Serialization
        public void LoadJson(JObject json)
        {
            PlayerMana = json["playerMana"].Value<int>();
            PlayerGold = json["playerGold"].Value<int>();
        }

        public JObject ToJson()
        {
            return new JObject(
                new JProperty("playerMana", PlayerMana),
                new JProperty("playerGold", PlayerGold)
            );
        }
        #endregion
    }
}