using ToB.Entities.Interface;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.Buffs
{
  [CreateAssetMenu(fileName = "new DamageDebuff", menuName = "Buff/Damage Debuff")]
  public class DamageDebuff : Buff, IAttacker
  {
    public float applyDamage = 0;
    public float applyDamageMultiplier = 1;
    public float effectDamage = 0;
    public float effectDamageMultiplier = 1;
    public float removeDamage = 0;

    public bool Blockable => false;
    public bool Effectable => false;
    public Vector3 Position { get; private set; }
    public Team Team => Team.None;

    public override void Apply(GameObject target, BuffInfo info)
    {
      Position = target.transform.position;
      target.Damage(applyDamage + info.level * applyDamageMultiplier, this);
    }
    public override void Effect(GameObject target, BuffInfo info)
    {
      Position = target.transform.position;
      target.Damage(effectDamage + info.level * effectDamageMultiplier, this);
    }

    public override void Remove(GameObject target)
    {
      Position = target.transform.position;
      target.Damage(removeDamage, this);
    }
  }
}