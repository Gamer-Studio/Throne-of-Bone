using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ToB.Utils
{
  [Serializable]
  public class RangedStat
  {
    public UnityEvent<float> onChanged;

    [SerializeField] [GetSet(nameof(Value))] private float value;
    [SerializeField] private Stat max;

    public RangedStat(float maxValue, float value, StatOperator<float> maxEffect = null)
    {
      max = new Stat(maxValue, maxEffect);
      Value = value;
    }

    public RangedStat(float maxValue) : this(maxValue, maxValue)
    {
    }

    public float Value
    {
      get => value;
      set
      {
        var prevValue = this.value;
        this.value = Math.Max(0, Math.Min(value, max));
        onChanged?.Invoke(prevValue);
      }
    }

    public Stat Max
    {
      get => max;
      set
      {
        max ??= new Stat(value);
        max.BaseValue = value;
        Value = this.value;
      }
    }

    public List<StatOperator<float>> MaxEffects => max.effects;
  }
}