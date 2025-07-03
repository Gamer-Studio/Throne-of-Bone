using UnityEngine;

namespace ToB.Entities.Obstacle
{
    public class MasterKeys : MonoBehaviour
    {

        [SerializeField] public bool isAcquired;
        //진행도 저장 불러오는 방법에 따라 프로퍼티화 하는 등 후처리해야 함. 일단 그냥 public 필드로.

        private void Awake()
        {
            isAcquired = false;
            //지금은 그냥 false 상태
            //진행도 상황을 받아 와서 isAcquired를 true/false해서 setActive 여부 결정
        }

        private void Start()
        {
            if (isAcquired) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isAcquired = true;
                Core.ResourceManager.Instance.GiveMasterKeyToPlayer();
                gameObject.SetActive(false);
            }
        }

    }
}