using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

namespace ToB.Behavior
{
  [Serializable, GeneratePropertyBag]
  [NodeDescription(
    name: "IntSwitch",
    description: "int 값에 따라 선택지를 선택합니다.", 
    icon: "Icons/Sequence",
    category: "Flow/Conditional")]
  public class IntSwitchComposite : Composite
  {
    [SerializeReference] public BlackboardVariable<int> input;

    protected override void OnEnd()
    {
      base.OnEnd();
      
      
    }
  }
}