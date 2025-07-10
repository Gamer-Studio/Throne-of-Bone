using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities
{
    public class SewerRatRoomSequence : MonoBehaviour
    {
        [Header( "보스 오브젝트" )]
        [SerializeField] private SewerRat firstSewerRat;
        [SerializeField] private List<SewerRat> anotherSewerRats;
        
        [Header("보스룸 로케이션")]
        [SerializeField] private Location roomLocation;
        [SerializeField] private Location ascendLocation;
        private LayerMask groundMask;
        

        [Header("보스룸 버추얼 카메라")]
        [SerializeField] private CinemachineVirtualCamera roomVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera ratVirtualCamera;
        
        CinemachineVirtualCamera mainVirtualCamera;
        private CinemachineBasicMultiChannelPerlin mainCamNoise;
        private CinemachineBasicMultiChannelPerlin roomCamNoise;
        

        private int phaseCount;
        private void Reset()
        {
            roomLocation = GetComponent<Location>();
        }

        private void Start()
        {
            // TODO: 진행도에서 이미 깼으면 스스스로 파괴. 최적화 하고자 하면 보스를 사전참조하지 않고 못 깼으면 그 때 인스턴스화
            // TODO: 다만 entity 오브젝트에 스케일 맞게 끌어놓는 게 편해서 일단 이렇게 해둡니다

            mainVirtualCamera = CameraHub.Instance.MainVirtualCamera;
            
            mainCamNoise = mainVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            roomCamNoise = roomVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            roomLocation.OnPlayerEntered += PlayerEntered;

            firstSewerRat.gameObject.SetActive(false);
            ratVirtualCamera.Follow = firstSewerRat.transform;
            foreach (var rat in anotherSewerRats)
            {
                rat.gameObject.SetActive(false);
            }
            
            groundMask = LayerMask.GetMask("Ground");
        }

        private void PlayerEntered()
        {
            if (phaseCount != 0) return;
            phaseCount++;
            InputManager.Instance.SetInputActive(false);
            StartCoroutine(Sequence());
        }

        private IEnumerator Sequence()
        {
            // 나중에 필요하면 이 본문을 1페이즈라는 이름의 코루틴으로
            yield return StartCoroutine(FirstRatEarthQuake());
            
            yield return new WaitForSeconds(1f);
            roomVirtualCamera.Priority = 0;
            ratVirtualCamera.Priority = 50;
            // DOTween.To(() => roomVirtualCamera.m_Lens.OrthographicSize,
            //     x => roomVirtualCamera.m_Lens.OrthographicSize = x, 12f, 1f);
            //
            yield return StartCoroutine(AscendAndLand(firstSewerRat));
            
            yield return new WaitForSeconds(1f);
            roomVirtualCamera.Priority = 0;

            firstSewerRat.target = StageManager.Instance.player.transform;
            InputManager.Instance.SetInputActive(true);
        }
        private IEnumerator FirstRatEarthQuake()
        {
            mainCamNoise.m_AmplitudeGain = 5f;
            mainCamNoise.m_FrequencyGain = 25f;
            roomCamNoise.m_AmplitudeGain = 5f;
            roomCamNoise.m_FrequencyGain = 25f;
            
            yield return new WaitForSeconds(1f);

            roomVirtualCamera.Priority = 50;
            roomVirtualCamera.transform.position = transform.position;
            roomVirtualCamera.transform.position += Vector3.down * 10f;
            roomVirtualCamera.m_Lens.OrthographicSize = 8f;
            
            SetSewerRatInGround(firstSewerRat);
            
            yield return new WaitForSeconds(2f);

            mainCamNoise.m_AmplitudeGain = 0;
            mainCamNoise.m_FrequencyGain = 0;
            roomCamNoise.m_AmplitudeGain = 0;
            roomCamNoise.m_FrequencyGain = 0;
        }

        
        private void SetSewerRatInGround(SewerRat sewerRat)
        {
            sewerRat.gameObject.SetActive(true);
            sewerRat.transform.position = ascendLocation.GetRandomPosition(true, true);
            sewerRat.Sprite.sortingOrder = -100;
            sewerRat.Animator.SetBool(EnemyAnimationString.Roll, true);
            sewerRat.Physics.gravityEnabled = false;
            sewerRat.Physics.collisionEnabled = false;
            
            CreateDust(sewerRat);
            
        }

        private void CreateDust(SewerRat sewerRat)
        {
            Vector2 rayOrigin = sewerRat.transform.position;
            rayOrigin.y = transform.position.y;
            Vector2 groundPoint = Physics2D.Raycast(rayOrigin, Vector2.down, 100f, groundMask).point;

            ParticleSystem groundDustEffect = sewerRat.Strategy.GroundDustEffect;
            groundDustEffect.transform.position = groundPoint;
            
            groundDustEffect.gameObject.SetActive(true);
            groundDustEffect.Play();
        }
        
        private IEnumerator AscendAndLand(SewerRat sewerRat)
        {
            sewerRat.Strategy.GroundDustEffect.gameObject.SetActive(false);
            
            ParticleSystem groundRubble = sewerRat.Strategy.GroundRubble;
            groundRubble.transform.position = sewerRat.Strategy.GroundDustEffect.gameObject.transform.position;
            
            groundRubble.gameObject.SetActive(true);
            groundRubble.Play();

            sewerRat.Physics.velocityY = sewerRat.DataSO.AscendPower * 1.3f;
            sewerRat.Physics.gravityEnabled = true;

            Vector3 scale = sewerRat.transform.localScale;
            scale.x = sewerRat.transform.position.x > StageManager.Instance.player.transform.position.x ? -1.5f:1.5f;
            sewerRat.transform.localScale = scale;

            yield return new WaitUntil(() => sewerRat.IsGrounded);
            ratVirtualCamera.Priority = 0;
            roomVirtualCamera.Priority = 50;
            roomVirtualCamera.transform.position = transform.position + Vector3.down * 5f;
            sewerRat.Animator.SetBool(EnemyAnimationString.Roll, false);
        }
    }
}
