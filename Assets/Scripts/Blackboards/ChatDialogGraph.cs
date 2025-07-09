using System.Collections.Generic;
using ToB.Blackboards.Nodes;
using UnityEngine;

namespace ToB.Blackboards
{
  [CreateAssetMenu(fileName = "new ChatDialog", menuName = "BlackBoard/Chat Dialog")]
  public class ChatDialogGraph : ScriptableObject
  {
    public List<BlackboardNode> nodes = new();
    public BlackboardNode startNode;
  }
}