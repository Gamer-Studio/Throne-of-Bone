using System;
using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using ToB.UI;
using ToB.Utils.Singletons;
using UnityEngine;

namespace ToB.Core
{
    public class GameCameraManager : Singleton<GameCameraManager>
    {
        [Header("게임 씬 기본 카메라")]
        [field:SerializeField] public Camera MainCamera { get; private set; }
        [field:SerializeField] public CinemachineBrain brainCam { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera MainVirtualCamera { get; private set; }

        [Header("기본 속성")]
        [SerializeField, ReadOnly] private float mainCamOriginalSize;
        [SerializeField] private float verticalStareDistance = 3;
        [SerializeField] private float stareSpeed = 1;
        
        [Header("현재 포커싱 중인 카메라")]
        [SerializeField, ReadOnly] private CinemachineVirtualCamera currentVirtualCamera;
        
        public float MainCamOriginalSize => mainCamOriginalSize;
        
        private CinemachineFramingTransposer mainTransposer;
        private Vector3 mainOffset;

        private bool UIResize;
        
        public float zoomRatio { get; private set; }

        private void Awake()
        {
            mainCamOriginalSize = MainCamera.orthographicSize;
            zoomRatio = 1;
            mainTransposer = MainVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            mainOffset = mainTransposer.m_TrackedObjectOffset;
        }

        void Update()
        {
            zoomRatio = MainCamera.orthographicSize / mainCamOriginalSize;
        }

        private void LateUpdate()
        {
            if (IsBlending() && UIResize) AdjustUISize();
            else if (currentVirtualCamera == MainVirtualCamera && UIManager.Instance.gamePlayUI.transform.localScale != Vector3.one)
            {
                if(Mathf.Abs(UIManager.Instance.gamePlayUI.transform.localScale.x - 1) < 0.001f)
                    UIManager.Instance.gamePlayUI.transform.localScale = Vector3.one;
            } 
        }

        /// <summary>
        /// 카메라 줌인아웃에 스크린 스페이스 오버레이인 UI도 반응하도록 유도합니다
        /// </summary>
        private void AdjustUISize()
        {
            float zoomCoef = mainCamOriginalSize / MainCamera.orthographicSize;
            UIManager.Instance.gamePlayUI.transform.localScale = new Vector3(zoomCoef, zoomCoef, zoomCoef);
        }

        /// <summary>
        /// 파라미터에 넣는 버추얼 카메라가 제일 높은 Priority를 갖습니다
        /// </summary>
        /// <param name="newCamera"></param>
        public void SwitchFirstCamera(CinemachineVirtualCamera newCamera, bool uiResize = true)
        {
            if (currentVirtualCamera) currentVirtualCamera.Priority = 0;
            currentVirtualCamera = newCamera;
            newCamera.Priority = 50;
            UIResize = uiResize;
        }
        
        /// <summary>
        /// 파라미터를 비울 경우 메인 카메라가 제일 높은 Priority를 갖습니다
        /// </summary>
        public void SwitchFirstCamera(bool uiResize = true)
        {
            SwitchFirstCamera(MainVirtualCamera, uiResize);
        }

        /// <summary>
        /// 버추얼 카메라간 전환되는 모션 시간을 설정합니다
        /// </summary>
        /// <param name="time"></param>
        public void SetBlendTime(float time)
        {
            brainCam.m_DefaultBlend.m_Time = time;
        }
        
        public bool IsBlending()
        {
            return brainCam.IsBlending;
        }

        public void Stare(float verticalInput)
        {
            if (verticalInput != 0)
            {
                if (verticalStareDistance > Mathf.Abs(mainOffset.y - mainTransposer.m_TrackedObjectOffset.y))
                {
                    mainTransposer.m_TrackedObjectOffset.y += verticalInput * stareSpeed;
                }
            }
            else
            {
                mainTransposer.m_TrackedObjectOffset.y = Mathf.Lerp(mainTransposer.m_TrackedObjectOffset.y, mainOffset.y, 0.1f);
            }
        }

        public void EarthQuake(float frequency, float amplitude, float duration)
        {
            CinemachineBasicMultiChannelPerlin perlin = currentVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = amplitude;
            perlin.m_FrequencyGain = frequency;

            DOVirtual.DelayedCall(duration, () =>
            {
                perlin.m_AmplitudeGain = 0;
                perlin.m_FrequencyGain = 0;
            });
        }
    }
}
