using UnityEngine;

namespace ToB.Entities.Obstacle
{
    public class OnOffDoor : MonoBehaviour
    {
       [SerializeField] public bool isOpened;
       [SerializeField] private SpriteRenderer DoorImage;
       //[SerializeField] private GameObject shadow;
       private Collider2D _collider;
       private void Awake()
       {
           // isOpened의 TF는 추후 세이브-로드에서 받아올 것
           _collider = GetComponent<Collider2D>();
           UpdateDoorState();
       }

       public void OnOffDoorInteract(bool leverState)
       {
           isOpened = leverState;
           UpdateDoorState();
       }

       private void UpdateDoorState()
       {
           DoorImage.color = isOpened ? Color.white : Color.yellow;
           _collider.enabled = !isOpened;
           //shadow.SetActive(!isOpened);
       }

    }
}
