using UnityEngine;

namespace ToB
{
    public class RoomLink : MonoBehaviour
    {
        // 스테이지 프리팹 인스턴스화 제도에서 링크가 다음 위치를 사전 참조할 수 없기 때문에 
        // ID를 기록하여 스테이지 이동 시 넘깁니다
        
        [SerializeField] public int Id { get; private set; }
        [SerializeField] private int linkedRoomID;      // 연결된 방의 ID
        [SerializeField] private int linkedOtherID;     // 연결된 방의 룸 링크 ID
        
        
    }
}
