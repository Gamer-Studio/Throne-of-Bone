using System.Collections.Generic;
using ToB.IO;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace ToB.Memories
{
    public class MemoriesManager : ManualSingleton<MemoriesManager>
    {
        //일지 데이터베이스
        public MemoriesDataSO memoriesDB;
        
        private Dictionary<int,bool> memoriesStates = new();
        //외부 참조용
        public Dictionary<int,bool> MemoriesStates => memoriesStates;
        
        //일지 습득 시 UI 갱신용 이벤트
        public UnityEvent<bool> onMemoryAcquired = new();

        private void Awake()
        {
            LoadMemoriesDataBase();
            InitializeMemoriesStates();
        }

        private void LoadMemoriesDataBase()
        {
            var handle = Addressables.LoadAssetAsync<MemoriesDataSO>("MemoriesDataBase");
            memoriesDB = handle.WaitForCompletion();
            if (memoriesDB != null) Debug.Log("일지 DB 어드레서블로 불러오기 성공");
        }
        private void InitializeMemoriesStates()
        {
            if (SAVE.Current == null)
            {
                Debug.Log("일지 세이브 파일이 없습니다.");
                foreach (var memory in memoriesDB.memoriesDataBase)
                {
                    memoriesStates.TryAdd(memory.id, false);
                }
                return;
            }

            memoriesStates = SAVE.Current.PlayerStat.savedMemoryStates;
        }

        public void MemoryAcquired(int _id)
        {
            if (memoriesStates[_id]) return; // true인 경우, 즉 이미 습득한 경우 리턴
            
            memoriesStates[_id] = true;
            onMemoryAcquired.Invoke(true);
        }
    }
}