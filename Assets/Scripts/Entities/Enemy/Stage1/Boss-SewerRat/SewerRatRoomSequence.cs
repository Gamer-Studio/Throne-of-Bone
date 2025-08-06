using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Entities.FieldObject;
using ToB.Scenes.Stage;
using ToB.Worlds;
using UnityEditor;
using UnityEngine;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities
{
    public class SewerRatRoomSequence : Room
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

        [Header("문")] 
        [SerializeField] private List<Door> doors;
        
        CinemachineVirtualCamera mainVirtualCamera;
        private CinemachineBasicMultiChannelPerlin mainCamNoise;
        private CinemachineBasicMultiChannelPerlin roomCamNoise;

        public bool hardMode;
        public int phaseCount;
        private void Reset()
        {
            roomLocation = GetComponent<Location>();
        }

        private void Start()
        {
            // TODO: 진행도에서 이미 깼으면 스스스로 파괴. 최적화 하고자 하면 보스를 사전참조하지 않고 못 깼으면 그 때 인스턴스화
            // TODO: 다만 entity 오브젝트에 스케일 맞게 끌어놓는 게 편해서 일단 이렇게 해둡니다

            mainVirtualCamera = StageManager.Instance.MainVirtualCamera;
            
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
            TOBInputManager.Instance.SetInputActive(false);
            StartCoroutine(Sequence());
        }

        private IEnumerator Sequence()
        {
            CloseDoors();
            yield return new WaitUntil(() => !doors[0].IsOpen);
            
            // Phase 1 : 한 마리
            yield return StartCoroutine(FirstRatEarthQuake());
            
            yield return new WaitForSeconds(1f);
            roomVirtualCamera.Priority = 0;
            ratVirtualCamera.Priority = 50;

            yield return StartCoroutine(AscendAndLand(firstSewerRat));
            
            yield return new WaitForSeconds(1f);
            roomVirtualCamera.Priority = 0;
            
            firstSewerRat.target = StageManager.Instance.player.transform;
            TOBInputManager.Instance.SetInputActive(true);
            
            // Phase 2 : 두 마리
            if(!hardMode) yield break;
            yield return new WaitUntil(() => !firstSewerRat.IsAlive);
            
            roomVirtualCamera.Priority = 50;
            roomVirtualCamera.m_Lens.OrthographicSize = 6f;
            
            SetSewerRatInGround(anotherSewerRats[0], ascendLocation.transform.position - new Vector3(ascendLocation.Width / 3, 0));
            
            SetSewerRatInGround(anotherSewerRats[1], ascendLocation.transform.position + new Vector3(ascendLocation.Width / 3, 0));
            
            yield return new WaitForSeconds(0.9f);
            
            StartCoroutine(AscendAndLand(anotherSewerRats[0]));
            
            yield return StartCoroutine(AscendAndLand(anotherSewerRats[1]));
            
            roomVirtualCamera.Priority = 0;
            yield return new WaitForSeconds(0.3f);
            
            anotherSewerRats[0].target = StageManager.Instance.player.transform;
            anotherSewerRats[1].target = StageManager.Instance.player.transform;
            
            yield return new WaitUntil(() => !anotherSewerRats[0].IsAlive && !anotherSewerRats[1].IsAlive);

            OpenDoors();
        }

        private void CloseDoors()
        {
            foreach (var door in doors)
            {
                door.Close();
            }
        }
        private void OpenDoors()
        {
            foreach (var door in doors)
            {
                door.Open();
            }
        }

        private IEnumerator FirstRatEarthQuake()
        {
            firstSewerRat.audioPlayer.Play("Movement_Earth_Loop_01", true);
            mainCamNoise.m_AmplitudeGain = 5f;
            mainCamNoise.m_FrequencyGain = 25f;
            roomCamNoise.m_AmplitudeGain = 5f;
            roomCamNoise.m_FrequencyGain = 25f;
            
            yield return new WaitForSeconds(1f);
            
            firstSewerRat.audioPlayer.Stop("Movement_Earth_Loop_01");
            roomVirtualCamera.Priority = 50;
            roomVirtualCamera.transform.position = transform.position + new Vector3(0,0,-10);
            roomVirtualCamera.transform.position += Vector3.down * 5f;
            roomVirtualCamera.m_Lens.OrthographicSize = 4f;
            
            SetSewerRatInGround(firstSewerRat, ascendLocation.transform.position);
            
            yield return new WaitForSeconds(2f);

            mainCamNoise.m_AmplitudeGain = 0;
            mainCamNoise.m_FrequencyGain = 0;
            roomCamNoise.m_AmplitudeGain = 0;
            roomCamNoise.m_FrequencyGain = 0;
        }

        
        private void SetSewerRatInGround(SewerRat sewerRat, Vector2 position)
        {
            sewerRat.gameObject.SetActive(true);
            sewerRat.transform.position = position;
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

            yield return new WaitUntil(() => sewerRat.Physics.velocityY < 0f);
            sewerRat.Physics.collisionEnabled = true;
            
            yield return new WaitUntil(() => sewerRat.IsGrounded);
            
            firstSewerRat.audioPlayer.Play("Footstep_05");
            firstSewerRat.audioPlayer.Play("AgressiveShout_04");
            ratVirtualCamera.Priority = 0;
            roomVirtualCamera.Priority = 50;
            roomVirtualCamera.transform.position = transform.position + Vector3.down * 2.5f + new Vector3(0,0,-10);
            sewerRat.Animator.SetBool(EnemyAnimationString.Roll, false);
        }
    }
}
