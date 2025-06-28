using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB
{
    public enum StageName
    {
        Proto
    }
    public class Stage : MonoBehaviour
    {
        [field:SerializeField] public StageName StageName { get; private set; }
        [field:SerializeField] public List<Room> Rooms { get; private set; }
        Dictionary<int, Room> RoomDic;

        
        private void Awake()
        {
            RoomDic = new Dictionary<int, Room>();
            foreach (Room room in Rooms)
            {
                RoomDic.Add(room.ID, room);
            }
        }

        public Room GetRoom(int roomID)
        {
            return RoomDic[roomID];
        }
    }
}
