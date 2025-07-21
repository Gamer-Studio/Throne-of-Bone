using System.Collections.Generic;
using ToB.IO;
using ToB.UI;
using ToB.Utils.Singletons;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace ToB.Entities.Skills
{
    public enum SkillState
    {
        Unacquired,
        Acquired,
        Deactivated
    }

    public class BattleSkillManager : ManualSingleton<BattleSkillManager>
    {
    public BattleSkillData skillDB;
    [SerializeField] private BattleSkillStats bsStats;
    public BattleSkillStats BSStats => bsStats;
    // 플레이어별 스킬 상태 관리(스킬 ID와 스킬 습득 여부)
    private Dictionary<int, SkillState> playerSkillStates = new();
    public Dictionary<int, SkillState> PlayerSkillStates => playerSkillStates;
    // 외부 참조용

    //UI 갱신용 이벤트
    public UnityEvent<int> onRangeAtkStackChanged = new();
    public UnityEvent<float> onMaxHpChanged = new();

    /// <summary>
    /// 정보를 불러오는 파트는 추후 Save-Load 연동 타이밍 결정되면 수정.
    /// 지금은 그냥 DB를 불러온 뒤 모든 스킬을 초기화하고 있습니다.
    /// </summary>
    private void Awake()
    {
        LoadSkillDataBase();
        InitializeSkillStates();
        ApplySkillsToPlayer();
    }

    
    /// <summary>
    /// CSV에서 파싱한 SO에서 정보를 불러옵니다.
    /// </summary>
    private void LoadSkillDataBase()
    {
        var handle = Addressables.LoadAssetAsync<BattleSkillData>("Assets/Data/BattleSkill/BattleSkillSO.asset");
        skillDB = handle.WaitForCompletion();
        if (skillDB != null) Debug.Log("스킬 DB 어드레서블로 불러오기 성공");
    }
    /// <summary>
    /// 불러온 정보를 바탕으로 초기에 플레이어의 스킬 정보 딕셔너리가 없는 경우 플레이어의 스킬 정보 딕셔너리를 기록합니다.
    /// 추후 초기화할 때도 이 메서드를 사용할 수 있습니다. (ResourceManager의 자원 환급 메서드와 함께 사용)
    /// </summary>
    private void InitializeSkillStates()
    {
        // 이게 불러오기
        playerSkillStates = SAVE.Current.PlayerStat.SavedPlayerSkillState;
        
        foreach (var skill in skillDB.BattleSkillDataBase)
        {
            playerSkillStates.TryAdd(skill.id, SkillState.Unacquired);
        }
    }

    /// <summary>
    /// NPC에게 스킬 초기화 시 호출할 메서드
    /// </summary>
    public void ResetSkillStates()
    {
        InitializeSkillStates();
        Core.ResourceManager.Instance.ReturnUsedResources();
    }

    public SkillState GetSkillState(int id)
    {
        if (playerSkillStates.TryGetValue(id, out var state))
        {
            return state;
        }
        else
        {
            Debug.LogWarning($"스킬 {id} 가 DB에 없습니다.");
            return SkillState.Unacquired;
        }
    }

    public bool LearnSkill(int id)
    {
        // 일단 id를 통해 Null처리부터
        if (!playerSkillStates.TryGetValue(id, out var skillState))
        {
            Debug.LogWarning($"스킬 {id} 가 DB에 없습니다.");
            return false;       
        }
        // Check 1. 이미 배운 스킬인지 확인
        if (skillState == SkillState.Acquired)
        {
            UIManager.Instance.toastUI.Show($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 는 이미 배운 스킬입니다.");
            return false;
        }
        // Check 2. 상위 티어 스킬을 배우려고 하는가? 추후 티어정보가 저장되는 곳이 확정되면...
        /*
        if (skillDB.GetSkillById(id).tier <= StageManager.tier)
        {
            UIManager.Instance.toastUI.Show($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 를 배우려면 티어가 더 높아야 합니다!");
            return false;
        }
        */
        // Check 3. 선행스킬을 찍었는지 체크.
        if (!CheckRequiredSkill(id))
        {
            UIManager.Instance.toastUI.Show("선행 스킬을 먼저 배워야 합니다.");
            return false;
        }
        
        int goldCost = skillDB.GetSkillById(id).goldCost;
        int manaCost = skillDB.GetSkillById(id).manaCost;
        // 비활성화된 스킬의 경우 찍을 때 골드가 들지 않습니다.
        if (GetSkillState(id) == SkillState.Deactivated) goldCost = 0;
        bool resourcePayed = Core.ResourceManager.Instance.IsPlayerHaveEnoughResources(goldCost, manaCost);
        if (resourcePayed)
        {
            // 스킬 활성화에 사용한 자원 카운트
            Core.ResourceManager.Instance.UsedGold += goldCost;
            Core.ResourceManager.Instance.UsedMana += manaCost;
            // 스킬의 상태를 배움 상태로 전환
            playerSkillStates[id] = SkillState.Acquired;
            // bsStat에 스탯 적용
            CategorizeSkills(id);
            // bsStat을 실제 플레이어에 적용
            bsStats.ApplyStats(bsStats);
            UIManager.Instance.toastUI.Show($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 획득!");
            return true;
        }
        else
        {
            UIManager.Instance.toastUI.Show($"자원이 부족하여 {skillDB.GetSkillById(id).skillName} 스킬을 습득하지 못했습니다.");
            return false;       
        }
    }

    /// <summary>
    /// 선행 스킬이 있는지 확인하는 메서드. 선행스킬 2개 모두 체크해서 둘 다 갖춰지면 true를 반환합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool CheckRequiredSkill(int id)
    {
        bool isAcquired = true;
        int requiredSkill_1 = skillDB.GetSkillById(id).reqID1;
        int requiredSkill_2 = skillDB.GetSkillById(id).reqID2;

        if (requiredSkill_1 == 0 || GetSkillState(requiredSkill_1) == SkillState.Acquired)
            isAcquired = true;
        else
        {
            isAcquired = false;
            return isAcquired;
        }
        if (requiredSkill_2 == 0 || GetSkillState(requiredSkill_2) == SkillState.Acquired)
            isAcquired = true;
        else
        {
            isAcquired = false;
            return isAcquired;       
        }
        
        return isAcquired;
    }

    /// <summary>
    /// 사망 시 호출하게 될, 배운 스킬을 모두 비활성화 상태로 전환하는 메서드입니다.
    /// </summary>
    public void DeactivateAllSkills()
    {
        foreach (var skill in skillDB.BattleSkillDataBase)
        {
            if (playerSkillStates[skill.id] == SkillState.Acquired)
                playerSkillStates[skill.id] = SkillState.Deactivated;
            ApplySkillsToPlayer();       
        }
    }

	
    /// <summary>
    /// Dict를 순회하며 모든 스킬을 스탯에 적용하는 메서드.
    /// 게임 시작 시 스킬 정보를 Load할 때 혹은 사망 시, 또는 스킬 초기화 시 호출됩니다.
    /// </summary>
    public void ApplySkillsToPlayer()
    {
        bsStats.ResetAllStats();
        foreach (var skill in playerSkillStates)
        {
            if (skill.Value == SkillState.Acquired)
                CategorizeSkills(skill.Key);
        }
        bsStats.ApplyStats(bsStats);
    }

    /// <summary>
    /// ID에 따라 적용할 메서드를 분류해 줍니다. 이거 수작업을 안 할 방법이...없는 것 같은데?
    /// </summary>
    /// <param name="id"></param>
    public void CategorizeSkills(int id)
    {
        if (id >= 10000 && id <= 10005)
            AttackUP(id);
        else if (id >= 10006 && id <= 10011)
            CritUP(id);
        else if (id >= 10012 && id <= 10014)
            Bleed(id);
        else if (id >= 10015 && id <= 10017)
            RangeAtkUP(id);
        else if (id == 10018)
            RangeAtkHeal(id);
        else if (id >= 20000 && id <= 20005)
            MaxHpUP(id);
        else if (id >= 20006 && id <= 20009)
            ParryUP(id);
        else if (id >= 20010 && id <= 20012)
            DefUP(id);
        else if (id >= 20013 && id <= 20016)
            DefGaugeUP(id);
        else if (id >= 30000 && id <= 30005)
            GoldUP(id);
        else if (id >= 30006 && id <= 30009)
            DashUP(id);
        else if (id >= 30010 && id <= 30012)
            ImmuneDebuff(id);
        else if (id >= 30013 && id <= 30015)
            Discount(id);
    }
    
    #region Skill Apply Methods

	private void AttackUP(int id)
    {
        bsStats.Atk += skillDB.GetSkillById(id).upStat1;
    }
    private void Discount(int id)
    {
        bsStats.DiscountShop += skillDB.GetSkillById(id).upStat1;
        bsStats.DiscountBlacksmith += skillDB.GetSkillById(id).upStat2;
    }
    private void ImmuneDebuff(int id)
    {
        switch (id)
        {
            case 30010:
                bsStats.IsImmuneByPoison = true;
                break;
            case 30011:
                bsStats.IsImmuneByFire = true;
                break;
            case 30012:
                bsStats.IsImmuneByElectric = true;
                break;
        }
    }
    private void DashUP(int id)
    {
        bsStats.DashImmuneTime += skillDB.GetSkillById(id).upStat1;
        bsStats.DashCooldown += skillDB.GetSkillById(id).upStat2;
    }
    private void GoldUP(int id)
    {
        bsStats.GoldUP += skillDB.GetSkillById(id).upStat1;
    }
    private void DefGaugeUP(int id)
    {
        bsStats.GuardGaugeDiscount += skillDB.GetSkillById(id).upStat1;
        bsStats.GuardGaugeRegen += skillDB.GetSkillById(id).upStat2;
    }
    private void DefUP(int id)
    {
        bsStats.Def += skillDB.GetSkillById(id).upStat1;
    }
    private void ParryUP(int id)
    {
        bsStats.ParryTime += skillDB.GetSkillById(id).upStat1;
        bsStats.ParryHealAmount += skillDB.GetSkillById(id).upStat2;
    }
    private void MaxHpUP(int id)
    {
        bsStats.MaxHp += skillDB.GetSkillById(id).upStat1;
        onMaxHpChanged.Invoke(bsStats.MaxHp);    
    }
    private void RangeAtkHeal(int id)
    {
        bsStats.RangeAtkHeal += skillDB.GetSkillById(id).upStat1;
    }
    private void CritUP(int id)
    {
        bsStats.CritChance += skillDB.GetSkillById(id).upStat1;
        bsStats.CritDmgMultiplier += skillDB.GetSkillById(id).upStat2;
    }
    private void Bleed(int id)
    {
        bsStats.BleedChance += skillDB.GetSkillById(id).upStat1;
        bsStats.BleedDmgMultiplier += skillDB.GetSkillById(id).upStat2;
    }
    private void RangeAtkUP(int id)
    {
        bsStats.RangeAtkStack += (int)skillDB.GetSkillById(id).upStat1;
        bsStats.RangeAtkDmgMultiplier += skillDB.GetSkillById(id).upStat2;
        onRangeAtkStackChanged.Invoke(bsStats.RangeAtkStack);
    }
    #endregion

    }
}