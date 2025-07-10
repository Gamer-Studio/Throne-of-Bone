using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace ToB.Behavior
{
  [Serializable, GeneratePropertyBag]
  [NodeDescription(
    name: "IntCondition",
    icon: "Icons/Sequence",
    category: "Action/Conditional")]
  public class IntCondition : Action
  {
    [SerializeReference] public BlackboardVariable<int> value;

    public IntCondition()
    {
      
    }
  }
}