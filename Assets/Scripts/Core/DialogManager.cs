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

        private bool talking;
        private void Awake()
        {
            talking = false;
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
            StageManager.Instance.ChangeGameState(GameState.Dialog);
            CurrentNPC = npc;
            
            UIManager.Instance.panelStack.Push(npc.DialogPanel);

            CurrentNPC.IsInteractable = false;
            brainCam.m_DefaultBlend.m_Time = 0.5f;
            FocusCameraToNPC();
            zoomCoroutine = StartCoroutine(ZoomInAndTalk());
        }

        
        public void CancelDialog()
        {
            if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            CurrentNPC.DialogPanel.gameObject.SetActive(false);
            talking = false;
            UIManager.Instance.panelStack.Pop();
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
            talking = true;
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
            CurrentNPC.Sprite.flipX = false;
            CurrentNPC.IsInteractable = true;
            CurrentNPC = null;
            StageManager.Instance.ChangeGameState(GameState.Play);
            
        }

        public void ProcessNPC()
        {
            if (!talking) return;
            if (brainCam.IsBlending) return;
            CurrentNPC.ProcessNext();
        }

        private void FocusCameraToNPC()
        {
            DialogCamera.Follow = CurrentNPC.transform;
            DialogCamera.m_Lens.OrthographicSize = CurrentNPC.ZoomSize;
            var transposer = DialogCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_TrackedObjectOffset = new Vector3(0, 2 * (CurrentNPC.ZoomSize / camOriginalSize), 0);
            DialogCamera.Priority = 30;
        }

        void DefocusCamera()
        {
            DialogCamera.Priority = 0;
        }
    }
}