using ToB.Entities;
using ToB.Utils;
using UnityEngine;

namespace ToB.Player
{
  public class PlayerAttackArea : MonoBehaviour
  {
    [SerializeField] private PlayerCharacter character;
    [SerializeField] private GameObject attackEffectPrefab;
    
    [SerializeField] LayerMask hittableLayers;

    private void Start()
    {
      hittableLayers = LayerMask.GetMask("Enemy");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    
      Vector2 otherCenter = other.GetComponent<Collider2D>().bounds.center;
      Vector2 posDiff = otherCenter - (Vector2)character.transform.position;
      Vector2 posDiffDir = posDiff.normalized;
      
      RaycastHit2D hit = Physics2D.Raycast(character.transform.position, posDiffDir, posDiff.magnitude, hittableLayers);
      
      if ((hittableLayers & 1 << other.gameObject.layer) != 0)
      {
        GameObject attackEffect = attackEffectPrefab.Pooling();
        attackEffect.transform.position = hit.point;
        
        float angle = Mathf.Atan2(posDiffDir.y, posDiffDir.x) * Mathf.Rad2Deg;
        
        var ps = attackEffect.GetComponent<ParticleSystem>().main;
        ps.startRotation = angle;
        
        attackEffect.gameObject.SetActive(true);
      }
      
      other.Damage(character.stat.atk, character);
      other.KnockBack(30, new Vector2(character.transform.eulerAngles.y == 0 ? 1 : -1, 0)); // 넉백 테스트했습니다. 머지할 때 의도하는 쪽으로 써주세요

    }
  }
}