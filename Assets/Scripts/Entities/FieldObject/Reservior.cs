using Newtonsoft.Json.Linq;
using ToB.Entities.Buffs;
using ToB.IO;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Reservior : FieldObjectProgress
    {
        public int WaterLevel;
        [SerializeField] public Transform[] WaterLevelPos;
        [SerializeField] public Lever[] levers;
        [SerializeField] public GameObject WaterBlock;

        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            WaterLevel = json.Get(nameof(WaterLevel), 0);
        }
        
        public override void OnLoad()
        {
            SetWaterLevel();
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(WaterLevel)] = WaterLevel;
            return json;
        }

        #endregion

        public void LeverInteraction()
        { 
            WaterLevel = 0;
            foreach (var lever in levers)
            {
                if (lever.isLeverActivated) WaterLevel++;
            }
            SetWaterLevel();
        }
        private void SetWaterLevel()
        {
            WaterBlock.transform.localPosition = WaterLevelPos[WaterLevel].localPosition;
           //WaterBlock.TryGetComponent<WaterObject>(out var water);
           //water.buffs[Buff.Poison].level = 5;
           //water.buffs[Buff.Poison].duration = 5;
           //water.buffs[Buff.Poison].delay = 1;
        }

    }
}