using System.Collections.Generic;
using UnityEngine;

namespace ToB.Entities.Skills
{
    [CreateAssetMenu(fileName = "BattleSkillSO", menuName = "Scriptable Objects/BattleSkillSO")]
    public class BattleSkillData : ScriptableObject
    {
        public List<SkillData> BattleSkillDataBase = new List<SkillData>();
        
        public SkillData GetSkillById(int id)
        {
            return BattleSkillDataBase.Find(skill => skill.id == id);
        }

        public SkillData GetSkillByName(string skillName)
        {
            return BattleSkillDataBase.Find(skill => skill.skillName == skillName);
        }
    }

    public enum SkillType
    {
        OFN = 1,
        DEF = 2,
        SUP = 3
    }
    
    [System.Serializable]
    public class SkillData
    {
        public int index;
        public int id;
        public string skillName;
        public int tier;
        public float upStat1;
        public float upStat2;
        public SkillType skillType;
        public int goldCost;
        public int manaCost;
        public int reqID1;
        public int reqID2;
    }
}
