using System.Collections;
using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class MovableBox : FieldObjectProgress, IKnockBackable
    {
        
        public bool IsRoomCleared;
        public Vector3 ClearedPosition;

        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            ClearedPosition = json.Get(nameof(ClearedPosition), transform.localPosition);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Set(nameof(ClearedPosition), ClearedPosition);
            return json;
        }
        #endregion
        
        public void SaveClearedBoxPos()
        {
            ClearedPosition = transform.localPosition;
            IsRoomCleared = true;
            BoxRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void TPBoxToPos(Vector3 pos)
        {
            gameObject.transform.position = pos;
        }
        
      
        
        #region KnockBack
        [SerializeField] public Rigidbody2D BoxRigidbody;
        [SerializeField] public float KnockBackForceMultiplier;
        public void KnockBack(float value, Vector2 direction)
        {
            if (IsRoomCleared) return;
            BoxRigidbody.AddForce(direction * value * KnockBackForceMultiplier, ForceMode2D.Impulse);
        }
        
        public void KnockBack(float value, GameObject sender)
        {
            Debug.Log("사용되지 않는 넉백");
        }
        #endregion

        
        private Coroutine pushImmuneCoroutine;
        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") && StageManager.Instance.player.IsDashing && !IsRoomCleared)
            {
                PushImmune();
            }
        }

        private void PushImmune()
        {
            if (pushImmuneCoroutine != null)
            {
                StopCoroutine(pushImmuneCoroutine);
                pushImmuneCoroutine = null;
            }
            pushImmuneCoroutine = StartCoroutine(PushImmuneByPlayer());
        }

        private IEnumerator PushImmuneByPlayer()
        {
            while (StageManager.Instance.player.IsDashing)
            {
                BoxRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                yield return new WaitForFixedUpdate();
            }
            BoxRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            pushImmuneCoroutine = null;
        }
    }
}