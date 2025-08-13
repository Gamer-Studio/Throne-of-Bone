using System;
using System.Collections;
using Cinemachine;
using ToB.Core;
using ToB.Entities;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.CutScene
{
    public abstract class DisposableCutScene : MonoBehaviour
    {
        private bool visited;
        private Location location;

        CinemachineVirtualCamera CurrentCamera => GameCameraManager.Instance.CurrentCamera;
        protected float savedZoomSize;
        private void Awake()
        {
            location = GetComponent<Location>();
            if (location) location.OnPlayerEntered += TriggerCutScene;
        }

        public void TriggerCutScene()
        {
            if (visited) return;
            visited = true;
            StartCoroutine(CutSceneCoroutine());
        }

        IEnumerator CutSceneCoroutine()
        {
            StageManager.Instance.ChangeGameState(GameState.CutScene);
            yield return StartCoroutine(CutSceneUnique());
            StageManager.Instance.ChangeGameState(GameState.Play);
        }
        
        protected abstract IEnumerator CutSceneUnique();

        private void OnDestroy()
        {
            if (location) location.OnPlayerEntered -= TriggerCutScene;
        }
        
        
        #region GrimoireOnly
        
        protected Grimoire Grimoire => StageManager.Instance.player.grimoire;

        protected IEnumerator GrimoireAppear()
        {
            savedZoomSize = CurrentCamera.m_Lens.OrthographicSize;  // 시퀀스 끝나고 돌려놓기 위해 킵
            yield return StartCoroutine(GameCameraManager.Instance.Zoom(3f, 1f));
            Grimoire.Appear();
            yield return new WaitForSeconds(Grimoire.MotionTime);

            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            yield return null;
        }

        protected IEnumerator GrimoireDisappear()
        {
            Grimoire.Disappear();
            yield return new WaitForSeconds(Grimoire.MotionTime);
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);
            yield return StartCoroutine(GameCameraManager.Instance.Zoom(savedZoomSize, 1f));
        }
        #endregion
    }
}
