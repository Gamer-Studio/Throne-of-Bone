using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToB
{
    public class World : MonoBehaviour
    {
        [field: SerializeField] public List<Stage> Stages { get; private set; }
        Dictionary<string, Stage> StageDic;      // 리스트에서 찾아도 금방일 것 같지만 혹시나 해서 마련함

        private void Awake()
        {
            StageDic = new Dictionary<string, Stage>();
            foreach (Stage stage in Stages)
            {
                StageDic.Add(stage.StageName, stage);
            }
        }
        
        public Stage GetStage(string stageName)
        {
            return StageDic[stageName];
        }
    }
}