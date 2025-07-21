using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ToB.Entities;
using ToB.Entities.Effect;
using ToB.Entities.Skills;
using ToB.IO;
using ToB.Utils;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ToB.Core
{
    public enum InfiniteResourceType
    {
        Gold,
        Mana
    }

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

        }

        public float GoldUP;
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
        
        [SerializeField] private int ResourcePerObject = 20;
        [SerializeField] private int maxPrefabCount = 10;

        /// <summary>
        /// 몬스터, 상자 등에서 자신의 위치를 중심으로 입력한 만큼의 자원이 튀어나오게 합니다.
        /// 자원 20당 1개의 구슬이 생성됩니다,
        /// </summary>
        /// <param name="자원 타입 - Gold / Mana "></param>
        /// <param name="드랍할 자원의 양"></param>
        /// <param name="드랍할 지점"></param>
        public void SpawnResources(InfiniteResourceType type, int resourceAmount, Transform spawnPoint)
        {
            //자원 오브젝트 생성할 수량 및 그 안에 들어갈 값 할당
            int prefabCount = Mathf.Clamp((resourceAmount / ResourcePerObject)+1, 1, maxPrefabCount);
            int resourceLeft = resourceAmount;
            // 자원 종류에 따라 프리펩 설정
            var prefabRef = type == InfiniteResourceType.Gold ? "Entities/GoldOrb" : "Entities/EnergyOrb";
            // 생성 위치를 살짝 랜덤하게(몬스터 위치에서 반경 0.2)
            Vector2 randomPos = (Vector2)spawnPoint.position + Random.insideUnitCircle * 0.2f;
            
            for (int i = 0; i < prefabCount; i++)
            {
                GameObject obj = PoolingHelper.Pooling(prefabRef, true);
                obj.transform.position = randomPos;
                obj.transform.rotation = Quaternion.identity;
                ResourceDropping resourceDropping = obj.GetComponent<ResourceDropping>();
                if (resourceLeft >= ResourcePerObject)
                {
                    resourceDropping.amount = ResourcePerObject;
                    resourceDropping.resourceType = type;
                }
                else
                {
                    resourceDropping.amount = resourceLeft;
                    resourceDropping.resourceType = type;
                }

                resourceLeft -= ResourcePerObject;
            }
        }

        /// <summary>
        /// 플레이어에게 골드를 줍니다. isGoldUpApplied는 골드 획득량 증가 옵션 적용 여부입니다(기본 true)
        /// 은행에서 출금하는 경우에는 false로 매개변수 넣어주시면 그대로 증가합니다.
        /// </summary>
        /// <param name="gold"></param>
        /// <param name="isGoldUPApplied"></param>
        public void GiveGoldToPlayer(int gold, bool isGoldUPApplied = true)
        {
            if (isGoldUPApplied) PlayerGold += (int)(gold * (1 + GoldUP));
            else PlayerGold += gold;
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
            //UsedGold += requiredGold;
            Debug.Log($"{requiredGold} 골드를 사용했습니다.");
        }
        
        /// <summary>
        /// 플레이어가 마력 결정 사용 시 호출
        /// </summary>
        /// <param name="requiredMana"></param>
        public void UseMana(int requiredMana)
        {
            PlayerMana -= requiredMana;
            //UsedMana += requiredMana;
            Debug.Log($"{requiredMana} 마나결정을 사용했습니다");
        }

        /// <summary>
        /// 스킬 초기화 시 사용한 골드를 돌려주는 메서드입니다.
        /// </summary>
        public void ReturnUsedResources()
        {
            GiveGoldToPlayer(UsedGold, false);
            GiveManaToPlayer(UsedMana);
            UsedGold = 0;
            UsedMana = 0;
        }
        
        
        #region Keys
        
        //public Dictionary<string, string> IndexedKey = new();
        // 추후 특정 문에만 맞는 키가 필요한 경우 로직 추가 - 딕셔너리 혹은 리스트?

        private int masterkey;
        public UnityEvent<int> onMasterKeyChanged = new();

        public int MasterKey
        {
            get => masterkey;

            set
            {
                masterkey = value;
                onMasterKeyChanged?.Invoke(masterkey);
            }
        }

        public void GiveMasterKeyToPlayer(int value = 1)
        {
            MasterKey += value;
        }
        
        public bool IsPlayerHaveEnoughMasterKey(int requiredKey = 1)
        {
            return MasterKey >= requiredKey;
        }

        public void UseMasterKey(int requiredKey = 1)
        {
            MasterKey -= requiredKey;
        }
        
        
        
        #endregion
        
        
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
            int totalGold = PlayerGold + UsedGold;
            PlayerGold = 0;
            UsedGold = 0;
            return totalGold;
        }
        public int DropAllManaToCorpse()
        {
            int totalMana = PlayerMana + UsedMana;
            PlayerMana = 0;
            UsedMana = 0;
            return totalMana;
        }
        
        #endregion
        
        #region Serialization
        public void LoadJson(JObject json)
        {
            PlayerMana = json.Get(nameof(playerGold), 0);
            PlayerGold = json.Get(nameof(playerMana), 0);
            MasterKey = json.Get(nameof(MasterKey), 0);
            UsedGold = json.Get(nameof(UsedGold), 0);
            UsedMana = json.Get(nameof(UsedMana), 0);
        }

        public JObject ToJson()
        {
            return new JObject(
                new JProperty(nameof(playerMana), PlayerMana),
                new JProperty(nameof(playerGold), PlayerGold),
                new JProperty(nameof(MasterKey), MasterKey),
                new JProperty(nameof(UsedGold), UsedGold),
                new JProperty(nameof(UsedMana), UsedMana)
            );
        }
        #endregion
    }
}