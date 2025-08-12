using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ToB.Entities.Interface;
using ToB.IO;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class Elevator : FieldObjectProgress, IInteractable, IDamageable
    {

        [Header("레버 및 엘레베이터")] [SerializeField]
        private List<Lever> levers = new List<Lever>();

        [SerializeField] private List<Transform> floorPos = new List<Transform>();
       
        [Header("스프라이트 관련")]
        // private TMPro.TextMeshProUGUI floorText;
        // private SpriteRenderer _spriteRenderer;
        // private Animator _animator;

        [Header("움직임 관련")] private Coroutine C_Move;
        private int floorIndex;

        [SerializeField] private float decelerationDistance;
        [SerializeField] private float acceleration;
        [SerializeField] private float maxSpeed;

        [Header("인터랙션 관련")] public bool IsInteractable { get; set; } = true;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] sprites; // 0 UP 1 Down 2 Moving
        [SerializeField] private bool WannaGoingUp;
        [SerializeField] private GameObject InitFloorPositions;
        private ObjectAudioPlayer audioPlayer;


       

        /// <summary>
        /// 레버를 당겼을 때 호출할 메서드.
        /// </summary>
        /// <param name="calledLever"></param>
        public void CallElevatorByLever(Lever calledLever)
        {
            if (calledLever == null) return;
            if (levers.IndexOf(calledLever) == floorIndex)
            {
                return;
            }

            floorIndex = levers.IndexOf(calledLever);
            UpdateLinkedLeverState(floorIndex);
            MoveToFloor();
        }

        /// <summary>
        /// 현재 floor와 엘베 위치에 맞게 연결된 레버의 상태를 전환
        /// </summary>
        /// <param name="_floorIndex"></param>
        public void UpdateLinkedLeverState(int _floorIndex)
        {
            foreach (var _lever in levers)
            {
                if (levers.IndexOf(_lever) != _floorIndex)
                {
                    _lever.isLeverActivated = false;
                    _lever.LeverStateUpdate();
                    _lever.IsInteractable = true;
                }
                else if (levers.IndexOf(_lever) == _floorIndex)
                {
                    _lever.isLeverActivated = true;
                    _lever.LeverStateUpdate();
                    _lever.IsInteractable = false;
                }

                _lever.interactionText.enabled = false;
            }
        }

        /// <summary>
        /// 엘베가 이동하는 코루틴을 시작합니다.
        /// </summary>
        private void MoveToFloor()
        {
            if (C_Move != null)
            {
                StopCoroutine(C_Move);
                IsInteractable = true;
                C_Move = null;
            }

            C_Move = StartCoroutine(ElevatorMove());
        }

        private IEnumerator ElevatorMove()
        {
            IsInteractable = false;
            audioPlayer.Play("Elevator");
            spriteRenderer.sprite = sprites[2];

            float speed = 0f;
            Vector3 startPos = transform.position;
            Vector3 endPos = floorPos[floorIndex].position;
            while (true)
            {
                Vector3 direction = (endPos - startPos).normalized;
                float distance = Vector3.Distance(transform.position, endPos);

                // 감속, 가속
                if (distance <= decelerationDistance)
                {
                    speed = Mathf.Lerp(speed, 0.1f, Time.deltaTime * acceleration * 0.3f);
                }
                else
                {
                    speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
                }

                transform.position += direction * (speed * Time.deltaTime);
                
                if (distance < 0.02f)
                {
                    transform.position = endPos;
                    break;
                }

                audioPlayer.Stop("Elevator");
                yield return new WaitForFixedUpdate();
            }
            spriteRenderer.sprite = sprites[WannaGoingUp ? 0 : 1];
            IsInteractable = true;
            C_Move = null;
        }

        #region SaveLoad

        private void UpdateElevatorPos()
        {
            transform.position = floorPos[floorIndex].position;
        }

        // 프로퍼티 : 세이브 로드 시에만 사용.
        public int FloorIndex
        {
            get => floorIndex;
            set
            {
                floorIndex = value;
                
                UpdateElevatorPos();
                UpdateLinkedLeverState(floorIndex);
            }
        }
        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            InitFloorPositions.transform.SetParent(gameObject.transform.parent);
            FloorIndex = json.Get(nameof(FloorIndex), 0);
        }

        public override void OnLoad()
        {
            WannaGoingUp = true;
            if (audioPlayer == null) audioPlayer = GetComponent<ObjectAudioPlayer>();
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(FloorIndex)] = floorIndex;
            return json;
        }


        #endregion

        public void Interact()
        {
            if (!IsInteractable) return;
            int temp = floorIndex;
            Debug.Log("상호작용 진행됨");

            switch (WannaGoingUp)
            {
                case true:
                    temp++;
                    if (temp >= floorPos.Count)
                    {
                        Debug.Log("더 윗층이 없습니다.");
                    }
                    else
                    {
                        floorIndex = temp;
                        UpdateLinkedLeverState(floorIndex);
                        MoveToFloor();
                    }
                    break;
                case false:
                    temp--;
                    if (temp < 0)
                    {
                        Debug.Log("더 아랫층이 없습니다.");
                    }
                    else
                    {
                        floorIndex = temp;
                        UpdateLinkedLeverState(floorIndex);
                        MoveToFloor();
                    }
                    break;
            }
        }

        public void Damage(float damage, IAttacker sender = null)
        {
            if (IsInteractable)
            {
                WannaGoingUp = !WannaGoingUp;
                spriteRenderer.sprite = sprites[WannaGoingUp ? 0 : 1];
            }
        }
    }
}