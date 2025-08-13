using System;

namespace ToB.Entities.Buffs
{
  [Serializable]
  public class BuffInfo
  {
    public int level = 0;
    public float duration = 0;
    public float delay, delayTime = 0;

    public BuffInfo(int level, float duration, float delay = 1)
    {
      this.level = level;
      this.duration = duration;
      this.delay = delay;
    }
    
    public BuffInfo Clone() => new (level, duration, delay);
  }
}