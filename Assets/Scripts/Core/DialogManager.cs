using System;
using System.Collections;
using Cinemachine;
using ToB.Entities.NPC;
using ToB.Scenes.Stage;
using ToB.UI;
using ToB.Utils.Singletons;
using Unity.AppUI.UI;
using UnityEngine;

namespace ToB.Core
{
    public class DialogManager:Singleton<DialogManager>
    {
        [field:SerializeField] public NPCBase CurrentNPC { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera DialogCamera { get; private set; }
        private CinemachineBrain brainCam;

        private Coroutine zoomCoroutine;
        
        private void Awake()
        {
            brainCam = Camera.main.GetComponent<CinemachineBrain>();
        }

        private void Reset()
        {
            DialogCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        public void StartDialog(NPCBase npc)
        {
            CurrentNPC = npc;
            StageManager.Instance.ChangeGameState(GameState.UI);
            //UIManager.Instance.
            FocusCameraToNPC();
            zoomCoroutine = StartCoroutine(TalkAfterCameraMotion());
        }

        
        public void CancelDialog()
        {
            if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            StageManager.Instance.ChangeGameState(GameState.Play);
            DefocusCamera();
        }

        IEnumerator TalkAfterCameraMotion()
        {
            while (!brainCam.IsBlending)
                yield return null;
            
            // 다이얼로그 UI 시작
        }
        
        private void FocusCameraToNPC()
        {
            DialogCamera.Follow = CurrentNPC.transform;
            DialogCamera.Priority = 30;
        }

        void DefocusCamera()
        {
            DialogCamera.Priority = 0;
            
        }
    }
}