using Newtonsoft.Json.Linq;
using ToB.IO;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class OnOffDoor : FieldObjectProgress
    {
       [SerializeField] public bool isOpened;
       [SerializeField] private SpriteRenderer DoorImage;
       private Collider2D _collider;
       
       #region SaveLoad

       public override void LoadJson(JObject json)
       {
           base.LoadJson(json);
           isOpened = json.Get(nameof(isOpened), false);
       }

       public override void OnLoad()
       {
           if (_collider ==null) _collider = GetComponent<Collider2D>();
           UpdateDoorState();
       }

       public override JObject ToJson()
       {
           JObject json = base.ToJson();
           json.Add(nameof(isOpened), isOpened);
           return json;
       }
       #endregion

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
