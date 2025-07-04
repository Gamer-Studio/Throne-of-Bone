using System;
using UnityEngine;

namespace ToB.Entities.Obstacle
{
    public class Reservior : MonoBehaviour
    {
        public int WaterLevel;
        [SerializeField] public Transform[] WaterLevelPos;
        [SerializeField] public Lever[] levers;
        [SerializeField] public GameObject WaterBlock;

        private void Awake()
        {
            WaterLevel = 0;
            // 추후 여기서 Load로 저장값 불러오기
            SetWaterLevel();
        }

        public void LeverInteraction()
        { 
            WaterLevel = 0;
            for (int i = 0; i < levers.Length; i++)
            {
                if (levers[i].isLeverActivated) WaterLevel++;
            }
            SetWaterLevel();
        }

        private void SetWaterLevel()
        {
            WaterBlock.transform.localPosition = WaterLevelPos[WaterLevel].localPosition;
        }

    }
}