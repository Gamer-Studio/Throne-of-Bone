using UnityEngine;

namespace ToB
{
    
    // 지금은 안 쓰고 적군이 방어력 버프를 쓰는 등 처리가 복잡해질 때를 감안해서 보류
    public class EnemyStatHandler : MonoBehaviour
    {
        private Enemy enemy;
        public float MaxHP { get; private set; }

        [SerializeField] private float currentHP;
        public float CurrentHP { get; private set; }

        [SerializeField] private float atk;
        public float ATK { get; private set; }

        public void Init(Enemy enemy)
        {
            this.enemy = enemy;
            MaxHP = enemy.EnemyData.HP;
            atk = enemy.EnemyData.ATK;
            // 필요하면 위 패턴 참고해서 초기화 예정
            
            currentHP = MaxHP;
        }
        
        public void ChangeHP(float delta)
        {
            CurrentHP += delta;
            CurrentHP = Mathf.Clamp(CurrentHP, 0, MaxHP);
        }
    }
}
