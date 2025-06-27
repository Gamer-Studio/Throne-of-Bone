using System;
using System.Collections;
using TMPro;
using ToB.Player; 
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private TMP_Text text;
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
    #region HPBarChange
    
    private PlayerCharacter player;
    
    private void Awake()
    {
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if (scene.name == "Stage" || scene.name == "Stage0623Copy")
      {
        gameObject.SetActive(true);
        player = PlayerCharacter.GetInstance();
        if (player != null)
        {
          Init();
        }
        else
        {
          Debug.Log("PlayerCharacter is null");
        }
        Debug.Log("HPBar Init end");
      }
    }
   /*
    private IEnumerator WaitAndInit()
    {
      Debug.Log("WaitAndInit");
      yield return new WaitForSeconds(0.5f);
      // 한 프레임 쉬거나 약간 딜레이 : UI가 DDO라서 플레이어보다 먼저 생성됨.
      // 추후 로딩신이 생겨서 await async등을 쓸 수 있으면 그때 더 정확하게 타이밍 조절할 수 있을 듯
      // player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerCharacter>();
      // 직접 찾아도 되는데 뭐 메서드 이미 있으니 아래 사용
      player = PlayerCharacter.GetInstance();
      if (player != null)
      {
        Init();
      }
      else
      {
        Debug.Log("PlayerCharacter is null");
      }
      Debug.Log("WaitAndInit end");
    }
    */
    private void Init()
    {
      ChangeMax(player.stat.maxHp);
      text.text = $"{player.stat.Hp} / {max}";
      UpdateHPBar(player.stat.Hp);
      player.stat.onHpChanged.AddListener(UpdateHPBar);
    }

    private void UpdateHPBar(float curHp)
    {
      Value = curHp;
      text.text = $"{curHp} / {max}";
    }
    
    #endregion
  }
}