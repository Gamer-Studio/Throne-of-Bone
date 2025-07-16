using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ToB.Entities.Skills
{
    public enum SkillState
    {
        Unacquired,
        Acquired,
        Deactivated
    }

    public class BattleSkillManager : MonoBehaviour
    {
    private BattleSkillData skillDB;
    // 플레이어별 스킬 상태 관리(스킬 ID와 스킬 습득 여부)
    private Dictionary<int, SkillState> playerSkillStates = new();
    public Dictionary<int, SkillState> PlayerSkillStates => playerSkillStates;

    private async void Awake()
    {
        await LoadSkillDataBase();

        InitializeSkillStates();
    }

    private async Task LoadSkillDataBase()
    {
        var handle = Addressables.LoadAssetAsync<BattleSkillData>("Assets/Data/BattleSkill/BattleSkillSO.asset");
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            skillDB = handle.Result;
            Debug.Log("스킬 DB 어드레서블로 불러오기 성공");
        }
        else
        {
            Debug.LogError("스킬 DB 어드레서블로 불러오기 실패");
        }
    }

    private void InitializeSkillStates()
    {
        foreach (var skill in skillDB.BattleSkillDataBase)
        {
            if (!playerSkillStates.ContainsKey(skill.id))
            {
                playerSkillStates.Add(skill.id, SkillState.Unacquired);
            }
        }
    }

    public void LearnSkill(int id)
    {
        // 일단 id를 통해 Null처리부터
        if (!playerSkillStates.ContainsKey(id))
        {
            Debug.LogWarning($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 가 DB에 없습니다.");
            return;       
        }
        // Check 1. 이미 배운 스킬인지 확인
        if (playerSkillStates[id] == SkillState.Acquired)
        {
            Debug.LogWarning($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 는 이미 배운 스킬입니다");
            return;
        }
        // Check 2. 상위 티어 스킬을 배우려고 하는가? 는 이거 게임매니저 같은 거가 필요하려나요? player가 저장해야 하나?
        /*
        if (skillDB.GetSkillById(id).tier <= GameManager.tier)
        {
            Debug.LogWarning($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 를 배우려면 티어가 더 높아야 합니다");
            return;
        }
        */
        // Check 3. 선행스킬을 찍었는지 체크.
        if (!CheckRequiredSkill(id)) return;
        
        int goldCost = skillDB.GetSkillById(id).goldCost;
        int manaCost = skillDB.GetSkillById(id).manaCost;
        // 비활성화된 스킬의 경우 찍을 때 골드가 들지 않습니다.
        if (GetSkillState(id) == SkillState.Deactivated) goldCost = 0;
        bool resourcePayed = Core.ResourceManager.Instance.IsPlayerHaveEnoughResources(goldCost, manaCost);
        if (resourcePayed)
        {
            playerSkillStates[id] = SkillState.Acquired;
            Debug.Log($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 획득"); 
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
        }
    }
    public SkillState GetSkillState(int id)
    {
        if (playerSkillStates.TryGetValue(id, out var state))
        {
            return state;
        }
        else
        {
            Debug.LogWarning($"스킬 {id} : {skillDB.GetSkillById(id).skillName} 가 DB에 없습니다.");
            return SkillState.Unacquired;
        }
    }


    }
}