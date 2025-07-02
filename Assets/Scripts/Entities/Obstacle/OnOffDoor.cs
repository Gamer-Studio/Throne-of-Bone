using UnityEngine;

namespace ToB.Entities.Obstacle
{
    public class OnOffDoor : MonoBehaviour
    {
       [SerializeField] private bool isOpened;
       [SerializeField] private SpriteRenderer DoorImage;
       private Collider2D _collider;
       private void Awake()
       {
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
       }

    }
}
