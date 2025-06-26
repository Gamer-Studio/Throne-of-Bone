using UnityEngine;

namespace ToB.Player
{
  public class PlayerCharacterObject : MonoBehaviour
  {
    [SerializeField] private PlayerCharacter character;

    /// <summary>
    /// 이벤트 트리거에요. 호출하지 말아주세요!
    /// </summary>
    public void AttackEnd()
    {
      character.AttackEnd();
    }
  }
}