using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{

    /// <summary>
    /// Normal : 레버가 On이면 문 열림, 레버가 Off이면 문 닫힘. 레버 하나만 할당 바람.
    /// Invert : 연결된 레버들 중 하니가 작동할 때마다 열림/닫힘 상태가 반전됨
    /// Or : 연결된 레버 중 하나라도 On인 경우 문이 열려 있음
    /// And : 연결된 레버 모두가 On일 경우에만 문이 열려 있음
    /// Fixed : 레버의 입력을 받아도 문의 열림닫힘 상태가 변하지 않음
    /// </summary>
    public enum DoorMode
    {
        Normal = 1,
        Invert = 2,
        Or = 3,
        And = 4,
        Fixed = 5
    }

    public class OnOffDoor : FieldObjectProgress
    {
       private readonly int Opened = Animator.StringToHash("IsOpened");
       [SerializeField] private List<Lever> levers = new List<Lever>();
       [SerializeField] private List<PressurePlate> pressurePlates = new List<PressurePlate>();
       
       private int activeInputCount;
       [SerializeField] private DoorMode doorMode;
       [SerializeField] private bool isOpened;
       [SerializeField] private Animator animator;
       private ObjectAudioPlayer audioPlayer;
       public bool IsOpened
       {
           get => isOpened;
           set
           {
               isOpened = value;
               UpdateDoorState();
           }
       }
       
       private Collider2D _collider;
       [SerializeField] private SpriteRenderer DoorImage;
       
       #region SaveLoad
       private void Awake()
       {
           if (_collider ==null) _collider = GetComponent<Collider2D>();
           audioPlayer = GetComponent<ObjectAudioPlayer>();
       }
       public override void LoadJson(JObject json)
       {
           base.LoadJson(json);
           doorMode = json.GetEnum(nameof(doorMode), doorMode);
           activeInputCount = json.Get(nameof(activeInputCount), activeInputCount);
           isOpened = json.Get(nameof(isOpened), isOpened);
           OnOffDoorInteract(isOpened);
       }
       public override void OnLoad()
       {
           OnOffDoorInteract(isOpened);
           // 세이브데이터 로딩이 끝나면 문의 상태를 결정해 줌
       }

       public override JObject ToJson()
       {
           JObject json = base.ToJson();
           json.Set(nameof(doorMode), doorMode);
           json[nameof(activeInputCount)] = activeInputCount;
           json[nameof(isOpened)] = isOpened;
           return json;
       }
       #endregion

       /// <summary>
       /// 레버에 의해 조작되면 doorMode에 따라 isOpened TF가 결정되고 이에 따라 문이 열리고 닫히는 메서드.
       /// </summary>
       /// <param name="leverState"></param>
       public void OnOffDoorInteract(bool leverState)
       {
           bool temp = isOpened;
           activeInputCount = 0;
           switch (doorMode)
           {
               case DoorMode.Normal:
                   activeInputCount = leverState ? 1 : 0;
                   IsOpened = activeInputCount > 0;
                   break;
               case DoorMode.Invert:
                   IsOpened = !IsOpened;
                   break;
               case DoorMode.Or:
                   foreach (var lever in levers) if (lever.isLeverActivated) activeInputCount++;
                   foreach (var plate in pressurePlates) if (plate.IsActivated) activeInputCount++;
                   IsOpened = activeInputCount > 0;
                   break;
               case DoorMode.And:
                   foreach (var lever in levers) if (lever.isLeverActivated) activeInputCount++;
                   foreach (var plate in pressurePlates) if (plate.IsActivated) activeInputCount++;
                   IsOpened = activeInputCount == levers.Count + pressurePlates.Count;
                   break;
               case DoorMode.Fixed:
                   break;
           }
           if (temp != IsOpened) audioPlayer.Play("Part_Assembly_30");
           UpdateDoorState();
       }
       
       /// <summary>
       /// isOpened값에 따라 문의 개폐상태를 적용함
       /// </summary>
       private void UpdateDoorState()
       {
           animator.SetBool(Opened, isOpened);
           _collider.enabled = !isOpened;
       }
       
       /// <summary>
       /// 도어모드 변경을 위한 메서드. fixed로만 전환하는 쪽으로 사용하는 게 좋을 것 같아요.
       /// </summary>
       /// <param name="mode"></param>
       public void ChangeDoorMode(DoorMode mode)
       {
           doorMode = mode;
       }

    }
}
