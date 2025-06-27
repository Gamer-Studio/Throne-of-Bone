using NaughtyAttributes;
using ToB.Player;
using ToB.Utils.Singletons;
using ToB.Worlds;
using UnityEngine;

namespace ToB
{
    public class WorldManager : Singleton<WorldManager>
    {
        [field: SerializeField] World world;
        [field: SerializeField] public int ElapsedMonth;    // 월드 안에 들어갈지 매니저가 들고있을지 고민되는 필드입니다
        [SerializeField] Season season;
        [field: SerializeField, ReadOnly] public Room CurrentRoom { get; private set; }
        [field: SerializeField] private PlayerCharacter player;

        void Start()
        {
            // 1. 진행도를 불러온다.
            // 2. 없으면 세계를 기본값으로 구성
            InitializeWorld();
        }

        private void InitializeWorld()
        {
            Debug.Log("WorldManager InitializeWorld");
            ElapsedMonth = 0;
            season = Season.Spring;
            
            // 일단 모든 스테이지 액티브 해제
            foreach (Stage stage in world.Stages)
            {
                stage.gameObject.SetActive(false);
            }
            
            CurrentRoom = world.GetStage("StageProto").GetRoom(0);  // 첫 스테이지 ID인 StageProto는 하이어라키에서 확인할 수 있습니다
            
            CurrentRoom.visited = true;
            CurrentRoom.parentStage.gameObject.SetActive(true);
            CurrentRoom.gameObject.SetActive(true);
            player.transform.position = new Vector3(-11.8800001f, -7.17000008f, 0); // 하이어라키에서 위치잡고 포지션값 복사한 그대로
        }
    }
}
