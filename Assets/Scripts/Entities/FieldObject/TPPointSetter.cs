using ToB.Player;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class TPPointSetter : MonoBehaviour
    {
        private Transform TPpoint;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
           TPpoint = transform.GetChild(0);
           spriteRenderer = GetComponent<SpriteRenderer>();
           spriteRenderer.enabled = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                TeleportPlayer(other.gameObject);
            }
        }

        private void TeleportPlayer(GameObject player)
        {
            StageManager.Instance.player.TPTransform = TPpoint;
            //ROOM 스케일링 문제가 해결되면, TPPointSetter 설치하고 코드 이걸로 변경
        }

    }
}