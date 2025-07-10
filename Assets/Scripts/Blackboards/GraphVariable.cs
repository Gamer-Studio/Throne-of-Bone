using System;

namespace ToB.Blackboards
{
  [Serializable]
  public class GraphVariable<T>
  {
    public T value;
    
    public GraphVariable(T value)
    {
      this.value = value;
      Unity.Behavior.BlackboardVariable<T> a;
    }
  }
}