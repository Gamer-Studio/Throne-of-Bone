using System;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Utils
{
  public delegate T StatOperator<T>(T origin);

  [Serializable]
  public class Stat
  {
    public UnityEvent<float> onChanged;
    [SerializeField] protected float baseValue;
    public StatOperator<float> effect;

    public Stat(float baseValue, StatOperator<float> effect = null)
    {
      this.baseValue = baseValue;
      this.effect = effect;
    }

    public virtual float BaseValue
    {
      get => baseValue;
      set
      {
        var prevValue = baseValue;
        baseValue = value;
        onChanged?.Invoke(prevValue);
      }
    }

    public virtual float Value => effect?.Invoke(baseValue) ?? baseValue;
    
    public override string ToString() => Value.ToString("F1");

    public static implicit operator float(Stat stat) => stat.Value;
    public static implicit operator Stat(float value) => new Stat(value);
    #region Operators

    public static Stat operator +(Stat stat, float value)
    {
      stat.BaseValue += value;
      return stat;
    }

    public static Stat operator -(Stat stat, float value)
    {
      stat.BaseValue -= value;
      return stat;
    }

    public static Stat operator *(Stat stat, float value)
    {
      stat.BaseValue *= value;
      return stat;
    }

    public static Stat operator /(Stat stat, float value)
    {
      stat.BaseValue /= value;
      return stat;
    }

    #endregion
  }
}