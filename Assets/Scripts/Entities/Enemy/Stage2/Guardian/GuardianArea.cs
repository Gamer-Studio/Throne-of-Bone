using System;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities
{
    public class GuardianArea : MonoBehaviour
    {
        [SerializeField] Location areaDetection;
        [SerializeField] Guardian guardian1;

        private void Awake()
        {
            areaDetection.OnPlayerEntered += PlayerEntered;
            areaDetection.OnPlayerExit += PlayerExit;
        }

        private void PlayerEntered()
        {
            guardian1.SetTarget(StageManager.Instance.player.transform);
        }

        private void PlayerExit()
        {
            guardian1.SetTarget(null);
            
        }
        
        private void OnDestroy()
        {
            areaDetection.OnPlayerEntered -= PlayerEntered;
            areaDetection.OnPlayerExit -= PlayerExit;
        }
    }
}
