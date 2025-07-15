using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB.Entities
{
    [CreateAssetMenu(fileName = "SewerRatSO", menuName = "Scriptable Objects/Enemy/Stage1/SewerRatSO")]
    public class SewerRatSO : 
        EnemySO,
        IEnemyHittableSO,
        IEnemyKnockbackSO
    {
        [field: SerializeField] public float HP { get; private set; }
        [field: SerializeField] public float DEF { get; private set; }
        [field: SerializeField] public float BodyDamage { get; private set; }
        [field: SerializeField] public float RollDamage { get; private set; }

        // 프로퍼티 SerializeField는 헤더 표시가 안 되서 분리하였습니다
        [Header("스크래치 패턴")] 
        [Tooltip("돌진 속도")]
        [SerializeField]
        private float dashSpeed;
        public float DashSpeed => dashSpeed;
        
        [Tooltip("돌진 지속시간")]
        [SerializeField]
        private float dashDuration;
        public float DashDuration => dashDuration;

        [Tooltip("할퀴기 대미지")]
        [SerializeField]
        private float scratchDamage;
        public float ScratchDamage => scratchDamage;
        
        [Tooltip("할퀴기 넉백")]
        [SerializeField]
        private float scratchKnockBackForce;
        public float ScratchKnockBackForce => scratchKnockBackForce;

        
        [Header("땅파기 패턴")]
        [Tooltip("공중으로 솟는 힘")]
        [SerializeField]
        private float ascendPower;
        public float AscendPower => ascendPower;
        
        [Tooltip("공중에서 플레이어에게 들이받는 속도")]
        [SerializeField]
        private float tackleSpeed;
        public float TackleSpeed => tackleSpeed;

        
        [Header("독뼈 던지기 패턴")]
        [Tooltip("뼈가 날아오는 속도")]
        [SerializeField]
        private float toxicBoneSpeed;
        public float ToxicBoneSpeed => toxicBoneSpeed;
        
        [Tooltip("뼈 던지는 시간 간격")]
        [SerializeField]
        private float toxicBoneInterval;
        public float ToxicBoneInterval => toxicBoneInterval;
        
        [field: SerializeField] public float KnockbackMultiplier { get; private set; } = 1;
        [field: SerializeField] public List<string> dialogs { get; private set; }  
    }
}
