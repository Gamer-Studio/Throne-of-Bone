using System;
using UnityEngine;

namespace ToB.Blackboards.Nodes
{
  [Serializable]
  public class BlackboardNode
  {
    #region Info
    
    public string name;
    public string description;
    
    #endregion
    
    [SerializeField] private BlackboardNode parent, child;

    public virtual bool SetParent(BlackboardNode parent)
    {
      this.parent = parent;
      return true;
    }

    public virtual BlackboardNode GetParent()
    {
      return parent;
    }
    
    public virtual bool SetChild(BlackboardNode child)
    {
      this.child = child;
      return true;
    }

    public virtual BlackboardNode GetChild()
    {
      return child;
    }
  }
}