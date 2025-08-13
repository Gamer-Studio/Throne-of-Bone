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
        
        private Coroutine zoomCoroutine;

        private bool talking;
        private void Awake()
        {
            talking = false;
        }

        private void Reset()
        {
            DialogCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        
        #region withNPC
        public void StartDialogWith(NPCBase npc)
        {
            CurrentNPC = npc;
            CurrentNPC.IsInteractable = false;

            StageManager.Instance.BeginDialog(npc);
            
            zoomCoroutine = StartCoroutine(SwitchCameraAndTalk());
        }

        
        public void CancelDialog()
        {
            if(zoomCoroutine != null) StopCoroutine(zoomCoroutine);
            CurrentNPC.DialogPanel.gameObject.SetActive(false);
            talking = false;
            UIManager.Instance.panelStack.Pop();

            zoomCoroutine = StartCoroutine(SwitchCameraAndQuit());
        }

        IEnumerator SwitchCameraAndTalk()
        {
            FocusCameraToNPC();
            yield return null;
            while(GameCameraManager.Instance.IsBlending()) yield return null;

            // 다이얼로그 UI 시작
            CurrentNPC.DialogPanel.gameObject.SetActive(true);
            talking = true;
            ProcessNPC();
        }

        IEnumerator SwitchCameraAndQuit()
        {
            GameCameraManager.Instance.SwitchFirstCamera();
            yield return null;
            while(GameCameraManager.Instance.IsBlending()) yield return null;
            
            CurrentNPC.Sprite.flipX = false;
            CurrentNPC.IsInteractable = true;
            CurrentNPC = null;
            StageManager.Instance.ChangeGameState(GameState.Play);
            
        }

        public void ProcessNPC()
        {
            if (!talking) return;
            if (GameCameraManager.Instance.IsBlending()) return;
            CurrentNPC.ProcessNext();
        }

        private void FocusCameraToNPC()
        {
            // 다이얼로그 카메라만의 속성은 다이얼로그 매니저가 여전히 책임집니다. 
            DialogCamera.Follow = CurrentNPC.transform;
            DialogCamera.m_Lens.OrthographicSize = CurrentNPC.ZoomSize;
            var transposer = DialogCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            transposer.m_TrackedObjectOffset = new Vector3(0, 2 * (CurrentNPC.ZoomSize / GameCameraManager.Instance.MainCamOriginalSize), 0);
            
            GameCameraManager.Instance.SwitchFirstCamera(DialogCamera);
        }
        
        #endregion
        
        
    }
}