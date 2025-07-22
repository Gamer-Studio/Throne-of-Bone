using System;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.ResourceManagement;

namespace ToB.Entities.Skills
{
    [Serializable]
    public class BattleSkillStats

    {

        [Header("파괴의 기억 추가 스텟")]
       
        public float Atk;
        public float CritChance;
        public float CritDmgMultiplier;
        public float BleedChance;
        public float BleedDmgMultiplier;

        public int RangeAtkStack;
        public float RangeAtkDmgMultiplier;
        public float RangeAtkHeal;
        
        [Header("수호의 기억 추가 스탯")]
        public float MaxHp;
        public float Def;
        public float ParryTime;
        public float ParryHealAmount;
        public float GuardGaugeDiscount;
        public float GuardGaugeRegen;

        [Header("전장의 기억 추가 스탯")]
        public float GoldUP;
        public float DashCooldown;
        public float DashImmuneTime;
        public float DiscountShop;
        public float DiscountBlacksmith;
        public bool IsImmuneByPoison;
        public bool IsImmuneByFire;
        public bool IsImmuneByElectric;

        /// <summary>
        /// 스킬로 인해 변한 스탯값을 게임플레이에 적용하는 메서드.
        /// </summary>
        /// <param name="stats"></param>
        public void ApplyStats(BattleSkillStats stats)
        {
            var player = StageManager.Instance.player;
            
            player.stat.tempAtk = Atk;
            player.stat.tempMaxHP = MaxHp;
            player.stat.tempDef = Def;
            
            // TODO : 출혈 관련은 디버프 미구현 관계로 적용 X
            
            // 크리티컬 확률, 데미지 적용은 PlayerAttackArea.cs에서 적용 중.
            // RangeAtkStack. Multiplier는 PlayerCharacterAttack.cs에서 해당 영역에서 직접 적용 중
            // RangeAtkHeal은 EnemyStatHandler.cs에서 적용 중
            // 패링 시간 완화, 패링시 체력 회복은 PlayerCharacterBlock.cs에서 적용 중
            // 가드 게이지 할인 및 회복속도 증가 또한 PlayerCharacterBlock.cs에서 적용 중
            // 대시 쿨타임 및 무적시간의 경우 PlayerCharacterDash.cs에서 적용 중
            
            Core.ResourceManager.Instance.GoldUP = GoldUP;
            // 상점 할인, 강화 할인 기능 미구현
            // 디버프 면역의 경우 BuffController.cs의 하단 fixedUpdate에 조건이 걸려 있음.
            
            // Comment(현) :
            //플레이어스탯으로 묶인 값들을 제외하고는 굳이 임시용 필드를 새로 만들어서 캐싱하기보다는
            //다이렉트로 기믹 적용 시 및 적용 조건에 추가하는 방식으로 스탯을 적용했습니다.
            //다이렉트로 기믹 로직에 박아버리느냐 vs 캐싱하느냐 기준은 해당 스탯이 사용되는 위치가 한정되어 있느냐- 였습니다.
            //일관된 방식이 아니라서 보기엔 불편할 수 있으나 굳이 캐싱을 해야 하나? 라는 생각이 들어서
            //이런 방식으로 구현했습니다.
        }
        
        public void ResetAllStats()
        {
            Atk = 0;
            CritChance = 0;
            CritDmgMultiplier = 0;
            BleedChance = 0;
            BleedDmgMultiplier = 0;
            RangeAtkStack = 0;
            RangeAtkDmgMultiplier = 0;
            RangeAtkHeal = 0;
            
            MaxHp = 0;
            Def = 0;
            ParryTime = 0;
            ParryHealAmount = 0;
            GuardGaugeDiscount = 0;
            GuardGaugeRegen = 0;
            
            GoldUP = 0;
            DashCooldown = 0;
            DashImmuneTime = 0;
            DiscountShop = 0;
            DiscountBlacksmith = 0;
            IsImmuneByPoison = false;
            IsImmuneByElectric = false;
            IsImmuneByFire = false;
        }
    }
}