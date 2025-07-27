using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using ToB.Core;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities
{
    public class GuardianArea : MonoBehaviour
    {
        [SerializeField] Location areaDetection;
        [SerializeField] Guardian guardian1;
        [SerializeField] Guardian guardian2;

        [SerializeField] CinemachineVirtualCamera roomCamera;
        [SerializeField] private GameObject prison;
        [SerializeField] private GameObject exitBlocker; 
        
        private List<Guardian> guardians;
        private void Awake()
        {
            areaDetection.OnPlayerEntered += PlayerEntered;
            areaDetection.OnPlayerExit += PlayerExit;
            
            guardians = new List<Guardian> {guardian1, guardian2}; // 방이 풀링되는 경우 Awake와 OnDestroy를 OnEnable/Disable로 
            guardians.ForEach(g => g.gameObject.SetActive(false));
            exitBlocker.SetActive(false);
        }

        private void PlayerEntered()
        {
            Sequence seq = DOTween.Sequence();

            seq.AppendInterval(1f);
            seq.AppendCallback(() => GameCameraManager.Instance.SwitchFirstCamera(roomCamera, false));
            seq.AppendCallback(() => StageManager.Instance.ChangeGameState(GameState.UI));
            seq.AppendInterval(1f);
            
            seq.AppendCallback(()=> exitBlocker.SetActive(true));
            seq.AppendCallback(() => prison.SetActive(false));
            seq.AppendCallback(() => GameCameraManager.Instance.EarthQuake(30,1,0.7f));
            
            seq.AppendInterval(1.5f);
            
            seq.AppendCallback(() => guardians.ForEach(g => g.gameObject.SetActive(true)));
            seq.AppendInterval(1f);
            
            seq.AppendCallback(() => GameCameraManager.Instance.SwitchFirstCamera(false));
            seq.AppendInterval(0.5f);
            seq.AppendCallback(() => StageManager.Instance.ChangeGameState(GameState.Play));
            
            seq.AppendCallback(()=> guardian1.SetTarget(StageManager.Instance.player.transform));
            seq.AppendInterval(1f);
            seq.AppendCallback(()=> guardian2.SetTarget(StageManager.Instance.player.transform));
            
            StartCoroutine(WaitUntilGuardiansDefeated(() =>
            {
                exitBlocker.SetActive(false);
            }));
        }

        IEnumerator WaitUntilGuardiansDefeated(Action callback)
        {
            yield return new WaitUntil(() => !guardian1.IsAlive && !guardian2.IsAlive);
            callback?.Invoke();
        }

        private void PlayerExit()
        {
            guardians.ForEach(g => g.SetTarget(null));
        }
        
        private void OnDestroy()
        {
            areaDetection.OnPlayerEntered -= PlayerEntered;
            areaDetection.OnPlayerExit -= PlayerExit;
        }

        public bool IsPointInArea(Vector2 point)
        {
            return areaDetection.IsPointInLocation(point);
        }
    }
}
