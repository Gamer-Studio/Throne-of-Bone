using System;
using UnityEngine;
using UnityEngine.UI;

namespace ToB.Utils.UI
{
  public class UIGaugeBar : MonoBehaviour
  {
    public enum FillDirection
    {
      Horizontal,
      Vertical
    }

    public enum FillMode
    {
      Rect,
      Image
    }

    public FillDirection fillDirection = FillDirection.Vertical;
    public FillMode fillMode = FillMode.Rect;
    public float max;
    [SerializeField] [GetSet("Value")] private float value;
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image image;

#if UNITY_EDITOR
    [SerializeField] [GetSet("Color")] private Color color;
#endif

    public float Value
    {
      get => value;
      set
      {
        this.value = value < 0 ? 0 : Math.Min(value, max);

        if (fillMode == FillMode.Rect)
          rect.localScale = fillDirection == FillDirection.Vertical
            ? new Vector3(1, this.value / max, 1)
            : new Vector3(this.value / max, 1, 1);
        else
          image.fillAmount = value / max;
      }
    }

    public Color Color
    {
      get => image.color;
      set => image.color = value;
    }

    public void ChangeMax(float maxValue)
    {
      max = maxValue;
      Value = Value;
    }
  }
}