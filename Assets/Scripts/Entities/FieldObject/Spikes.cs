using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private int damage;
        public Transform SpikeTPTransform;
        
        private void TeleportPlayer(GameObject player)
        {
            if (!SpikeTPTransform)
            {
                Debug.LogWarning("SpikeTPTransform이 null입니다.\n 플레이어가" +
                                 "SpikeTPPointSetter의 콜라이더를 거치지 않고\n" +
                                 "가시에 찔렸는지 확인해 주세요.");
                return;
            }
            // TODO : 가시 오브젝트에 닿은 순간 플레이어의 속력을 0으로 만들고, 상태 또한 IDLE로 전환해줘야 하고, 순간이동 시 이런저런 연출도 추가가 필요함
            player.transform.position = SpikeTPTransform.position;
        }
        
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.Damage(damage,this);
                TeleportPlayer(other.gameObject);
                //StageManager.Instance.player.TeleportByObstacle();
            }
        }
    }
}