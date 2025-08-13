using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Reservior : FieldObjectProgress
    {
        public int WaterLevel;
        [SerializeField] public Transform[] WaterLevelPos;
        [SerializeField] public GameObject WaterBlock;
        private JObject _lever1;
        private JObject _lever2;
        private bool _isLever1Activated;
        private bool _isLever2Activated;

        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            WaterLevel = json.Get(nameof(WaterLevel), 2);
            WaterBlock.transform.localPosition = WaterLevelPos[WaterLevel].localPosition;
        }
        
        public override void OnLoad()
        {
            _lever1 = Room.GetData(1, 12, "Lever");
            _lever2 = Room.GetData(1, 14, "Lever");
            
            _isLever1Activated = _lever1.Get("isLeverActivated", false);
            _isLever2Activated = _lever2.Get("isLeverActivated", false);
            
            SetWaterLevel();
        }
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(WaterLevel)] = WaterLevel;
            return json;
        }

        #endregion

        private void SetWaterLevel()
        {
            WaterLevel = 2;
            if (!_isLever1Activated) WaterLevel--;
            if (!_isLever2Activated) WaterLevel--;
            WaterBlock.transform.localPosition = WaterLevelPos[WaterLevel].localPosition;
        }

    }
}