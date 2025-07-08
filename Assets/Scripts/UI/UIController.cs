using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ToB.UI
{
    public class UIController : MonoBehaviour
    {
        private Camera _camera;
        private InputActionAsset InputActionAsset;

        [field:SerializeField] public UIPanelBase CurrentUI { get; private set; }
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // scene이 재로드될 때마다 playerInput 컴포넌트에 필요한 것들 할당
        // InputActionAsset은 처음 씬에 배치할 프리펩에서 할당
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Init(scene.name);
        }

        private void Init(string sceneName)
        {   
            // // 씬의 카메라 할당
            // _camera = Camera.main;
            // // 널처리
            // if (playerInput == null) playerInput = GetComponent<PlayerInput>();
            // // 인스펙터에서 끌어다 넣은 에셋 캐싱
            // InputActionAsset = playerInput.actions;
            // playerInput.camera = _camera;
            // //Auto-Switch 끄기 : 오토 스위치가 켜져 있으면 자동으로 액션맵을 스위칭할 위험이 있음
            // playerInput.neverAutoSwitchControlSchemes = true;
            // //유니티 이벤트 인보크로 설정
            // playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
            // //이벤트 시스템의 UI Input Module은 싱글플레이의 경우 할당하지 않아도 무관
            // SetTargetActionMap(sceneName);
        }

        private void SetTargetActionMap(string _sceneName)
        {
            // //에셋의 액션맵들을 다 disable한 뒤, 특정 액션 맵만 켜 주도록 함(액션 맵 이름 = 플레이어 말고는 씬 이름)
            // foreach (var actionMap in InputActionAsset.actionMaps)
            // {
            //     actionMap.Disable();
            // }
            //
            // //추후 씬 정리되거나 액션맵이 추가될 경우 이 부분 수정 필요.
            // if (_sceneName == "MainMenu")
            // {
            //     playerInput.currentActionMap = InputActionAsset.FindActionMap(_sceneName);
            // }
            // else
            // {
            //     playerInput.currentActionMap = InputActionAsset.FindActionMap("Stage");
            // }
            // playerInput.currentActionMap.Enable();
            // Debug.Log(playerInput.currentActionMap.name);
            // /*
            // 이 부분을 리플렉션 + 델리게이트로 액션 목록을 받아와서 등록하는 자동화도 해볼까 했는데, 제 수준에 안 맞기도 하고
            // 너무 추상화가 많이 들어가서 가독성도 별로일 것 같아서 (이벤트 구독 해제 과정도 쉽지 않을 것 같아서)
            // Action Map의 action들을 불러와서 이벤트를 구독하는 과정은 인스펙터에서 진행하려고 합니다.
            //
            // if (playerInput.currentActionMap.name == "MainMenu")
            // {
            //     var escAction = playerInput.currentActionMap.FindAction("CloseCurrentPanel");
            //     escAction.performed += ctx => UIManager.Instance.introUI.CloseCurrentPanel(ctx);
            //     escAction.Enable();
            // }
            // else if (playerInput.currentActionMap.name == "Stage")
            // {
            //     Debug.Log("Stage Action Map의 액션들 이벤트 할당 시작");
            //     // 아이고 언제 다 한다냐
            // }
            // */
        }
        
        public void CancelCurrentPanel()
        {
            CurrentUI?.Cancel();
        }
    }
}