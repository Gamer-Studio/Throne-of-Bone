using ToB.Entities;
using UnityEngine;

namespace ToB.Player
{
  public class PlayerAttackArea : MonoBehaviour
  {
    [SerializeField] private PlayerCharacter character;

    private void OnTriggerEnter2D(Collider2D other)
    {
      other.Damage(character.stat.atk, character);
      other.KnockBack(30, new Vector2(character.transform.eulerAngles.y == 0 ? 1 : -1, 0)); // 넉백 테스트했습니다. 머지할 때 의도하는 쪽으로 써주세요
      //other.KnockBack(30, gameObject);
      
    }
  }
}