using Newtonsoft.Json.Linq;
using ToB.Core;
using ToB.Entities;
using ToB.IO;
using UnityEngine;

namespace ToB.World.Structures
{
  public class DeathObject : Structure, IInteractable
  {
    public int deathVersion;
    public int gold;
    public int mana;
    [field: SerializeField] public bool IsInteractable { get; set; } = true;
    [SerializeField] private GameObject guideText;
    
    #region Unity Event

    private void OnTriggerEnter2D(Collider2D other)
    {
      if (other.CompareTag("Player"))
        guideText.SetActive(true);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
      if (other.CompareTag("Player"))
        guideText.SetActive(false);
    }
    
    #endregion
    
        
    public void Interact()
    {
      if(deathVersion == SAVE.Current.Player.deathVersion)
        ResourceManager.Instance.PlayerGold += gold;
      ResourceManager.Instance.PlayerMana += mana;
      
      Destroy(gameObject);
    }
    
    #region Serialization

    public override JObject ToJson()
    {
      var data = base.ToJson();
      
      data[nameof(gold)] = gold;
      data[nameof(mana)] = mana;
      data[nameof(deathVersion)] = deathVersion;
      
      return data;
    }

    public override void LoadJson(JObject json)
    {
      base.LoadJson(json);
      gold = json.Get(nameof(gold), gold);
      mana = json.Get(nameof(mana), mana);
      deathVersion = json.Get(nameof(deathVersion), deathVersion);
    }

    #endregion
  }
}