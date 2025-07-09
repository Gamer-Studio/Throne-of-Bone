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
        
        private Camera mainCamera;
        private CinemachineBrain brainCam;
        private float camOriginalSize;
        
        private Coroutine zoomCoroutine;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            brainCam = mainCamera.GetComponent<CinemachineBrain>();
            camOriginalSize = Camera.main.orthographicSize;
        }

        private void Reset()
        {
            DialogCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        public void StartDialogWith(NPCBase npc)
        {
            CurrentNPC = npc;
            StageManager.Instance.ChangeGameState(GameState.UI);
            UIManager.Instance.panelStack.Push(npc.DialogPanel);
            FocusCameraToNPC();
            zoomCoroutine = StartCoroutine(ZoomInAndTalk());
        }

        
        public void CancelDialog()
        {
            Debug.Log("CancelDialog");
            if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            StageManager.Instance.ChangeGameState(GameState.Play);
            CurrentNPC.DialogPanel.gameObject.SetActive(false);
            CurrentNPC = null;
            DefocusCamera();

            zoomCoroutine = StartCoroutine(ZoomOut());
        }

        IEnumerator ZoomInAndTalk()
        {
            yield return null;
            
            while (brainCam.IsBlending)
            {
                float zoomCoef = camOriginalSize / mainCamera.orthographicSize;
                UIManager.Instance.gamePlayUI.transform.localScale = new Vector3(zoomCoef, zoomCoef, zoomCoef);
                yield return null;
            }
            
            // 다이얼로그 UI 시작
            CurrentNPC.DialogPanel.gameObject.SetActive(true);
            ProcessNPC();
        }

        IEnumerator ZoomOut()
        {
            yield return null;

            while (brainCam.IsBlending)
            {
                float zoomCoef = camOriginalSize / mainCamera.orthographicSize;
                UIManager.Instance.gamePlayUI.transform.localScale = new Vector3(zoomCoef, zoomCoef, zoomCoef);
                yield return null;
            }
        }

        public void ProcessNPC()
        {
            if (brainCam.IsBlending) return;
            CurrentNPC.ProcessNext();
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