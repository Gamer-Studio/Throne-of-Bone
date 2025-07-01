using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Worlds
{
  public class Room : MonoBehaviour
  {
    [ContextMenuItem("내부 링크 찾기", nameof(FindLinks))]
    public List<RoomLink> links;
    public Behaviour[] behaviours;

    public UnityEvent onLoad = new();
    public UnityEvent onUnload = new();
    
    #if UNITY_EDITOR

    private void FindLinks()
    {
      
    }

    private void FindLinks(Transform tr)
    {
      for (var i = 0; i < tr.childCount; i++)
      {
        var child = tr.GetChild(i);
        if (child.TryGetComponent<RoomLink>(out var link))
          links.Add(link);
        FindLinks(child);
      }
    }
    
    #endif
  }
}