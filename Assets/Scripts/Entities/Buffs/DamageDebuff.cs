using UnityEngine;

namespace ToB.Entities.Buffs
{
  [CreateAssetMenu(fileName = "new DamageDebuff", menuName = "Buff/Damage Debuff")]
  public class DamageDebuff : Buff
  {
    public float applyDamage = 0;
    public float applyDamageMultiplier = 1;
    public float effectDamage = 0;
    public float effectDamageMultiplier = 1;
    public float removeDamage = 0;
    
    public override void Apply(GameObject target, BuffInfo info) 
      => target.Damage(applyDamage + info.level * applyDamageMultiplier);
    public override void Effect(GameObject target, BuffInfo info) 
      => target.Damage(effectDamage + info.level * effectDamageMultiplier);

    public override void Remove(GameObject target)
      => target.Damage(removeDamage);
  }
}