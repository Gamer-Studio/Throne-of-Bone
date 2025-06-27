using System;
using Cinemachine;
using UnityEngine;

namespace ToB
{
    public class Room : MonoBehaviour
    {
        [field:SerializeField] public Stage parentStage { get; private set; }
        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [field:SerializeField] public int ID { get; private set; }
        
        public bool visited;

        private void Awake()
        {
            if(!parentStage) parentStage = GetComponentInParent<Stage>();
            if (!virtualCamera) virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        private void Reset()
        {
            parentStage = GetComponentInParent<Stage>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }
    }
}
