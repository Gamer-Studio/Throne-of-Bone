using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "FlySO", menuName = "Scriptable Objects/Enemy/Stage1/FlySO")]
    public class FlySO : 
        EnemySO,
        IEnemyHittableSO,
        IConatactDamageSO,       // 파리 본체가 아니라 Sting 투사체 정보를 위한 인터페이스입니다. Sting용 SO가 분리되면 구조상 깔끔했겠지만 참조가 성가셔서 여기에 붙입니다.
        IEnemySightSensorSO,
        IEnemyKnockbackSO
    {
        [field:SerializeField] public float HP { get; private set; }
        [field:SerializeField] public float DEF { get; private set; }
        [field:SerializeField] public float ATK { get; private set; }
        [field:SerializeField] public float AttackDelay { get; private set; }
        [field:SerializeField] public float StingSpeed { get; private set; }
        [field:SerializeField] public float KnockbackPower { get; private set; }
        [field:SerializeField] public float DefaultMovementSpeed { get; private set; }
        [field:SerializeField] public float FastMovementSpeed { get; private set; }
        [field:SerializeField] public float KnockbackMultiplier { get; private set; }
        
        [field:SerializeField] public float SightRange { get; private set; }
        [field:SerializeField] public float SightAngle { get; private set; }
        
        [field:SerializeField] public float AttackRange { get; private set; }
        
        /// <summary>
        /// 이 댐핑은 지금은 쫓다가 공격하는 상태에만 적용되고 복귀, 패트롤 등에는 해당 사항이 없습니다
        /// </summary>
        [field:SerializeField] public float FlyDamping { get; private set; }
        
    }
}
