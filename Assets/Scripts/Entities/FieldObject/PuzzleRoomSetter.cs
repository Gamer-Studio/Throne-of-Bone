using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Scenes.Stage;
using ToB.UI;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class PuzzleRoomSetter : FieldObjectProgress, IInteractable
    {
        [Header("방 안의 오브젝트들 할당")]
        //Find해도 되긴 하는데, 어차피 프리팹 사용하는 상황에 굳이 방 내부 오브젝트 순회하면서 찾을 필요 없을 것 같아서.
        [SerializeField] public List<MovableBox> BoxesInRoom = new List<MovableBox>();
        [SerializeField] public List<Lever> LeversInRoom = new List<Lever>();
        [SerializeField] public List<PressurePlate> PlatesInRoom = new List<PressurePlate>();
        
        private List<Vector3> BoxesInitPos = new List<Vector3>();
        
        [Header("상호작용 및 초기화")]
        [SerializeField] private Transform PlayerTPpos;
        [SerializeField] private TMPro.TextMeshProUGUI interactionText;
        public bool IsCleared;
        public bool IsInteractable { get; set; } = true;
        
        #region SaveLoad

        
        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            IsCleared = json.Get(nameof(IsCleared), false);
            
            // 박스 초기 위치 저장
            foreach (var box in BoxesInRoom) BoxesInitPos.Add(box.transform.position);
        }
        
        public override void OnLoad()
        {
            if (IsCleared) MoveBoxToClearedPos();
        }

        
        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json[nameof(IsCleared)] = IsCleared;
            return json;
        }
        #endregion

        // 클리어 시 호출됨! 클리어 여부는 콜라이더가 판정해 줌.
        public void Interact()
        {
            if (!IsCleared) ResetRoom();
        }
        /// <summary>
        /// 방을 리셋시킬 때 적용할 메서드. 미 클리어 상태에서 오브젝트와 상호작용 시 작동함.
        /// </summary>
        private void ResetRoom()
        {
            if (IsCleared) return;
            
            UIManager.Instance.toastUI.Show("퍼즐 리셋");

            // 플레이어를 지정한 위치로 순간이동시킴
            StageManager.Instance.player.TPTransform = PlayerTPpos;
            StageManager.Instance.player.TeleportByObject();
            
            // 박스의 위치를 초기 설정한 위치로 이동
            foreach (var box in BoxesInRoom)
            {
                box.TPBoxToPos(BoxesInitPos[BoxesInRoom.IndexOf(box)]);
                box.BoxRigidbody.linearVelocity = Vector2.zero;
            }

            // 레버의 상태를 초기 미작동 상태로 돌리기
            foreach (var lever in LeversInRoom)
            {
                lever.isLeverActivated = false;
                lever.LeverStateUpdate();
            }
            
            
        }
        public void ClearRoom()
        {
            UIManager.Instance.toastUI.Show("퍼즐 클리어 완료!");
            IsCleared = true;
            // 박스들의 현재 위치를 저장하고 박스의 상태를 고정시킴
            foreach (var box in BoxesInRoom) box.SaveClearedBoxPos();
            // 레버의 상태를 고정하기 위해 상호작용을 금지시킴
            foreach (var lever in LeversInRoom) lever.IsInteractable = false;
            // 발판 또한 상태를 고정함
            foreach (var plate in PlatesInRoom) plate.Clear();
        }
        private void MoveBoxToClearedPos()
        {
            foreach(var box in BoxesInRoom) box.TPBoxToPos(box.ClearedPosition);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsCleared && other.CompareTag("Player")) interactionText.text = "F : 초기화";
            else if (IsCleared && other.CompareTag("Player")) interactionText.text = "클리어 완료";
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) interactionText.text = "";
        }
       

    }
}