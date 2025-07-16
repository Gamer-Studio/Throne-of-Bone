using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB.Entities.Skills
{
    public enum SkillState
    {
        Unacquired,
        Acquired,
        Deactivated
    }

    public class BattleSkill : MonoBehaviour
    {
        //KEY값은 스킬트리, Enum은 SkillState
        public Dictionary<string, Enum> OffensiveSkillList = new Dictionary<string, Enum>();
        public Dictionary<string, Enum> DefensiveSkillList = new Dictionary<string, Enum>();
        public Dictionary<string, Enum> SupportiveSkillList = new Dictionary<string, Enum>();
        
        
        
        
    }
}