using Newtonsoft.Json.Linq;
using ToB.IO;
using ToB.UI;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class MasterKeys : FieldObjectProgress
    {

        [SerializeField] public bool isAcquired;
        //진행도 저장 불러오는 방법에 따라 프로퍼티화 하는 등 후처리해야 함. 일단 그냥 public 필드로.

        #region SaveLoad

        public override void LoadJson(JObject json)
        {
            base.LoadJson(json);
            isAcquired = json.Get(nameof(isAcquired), false);
        }

        public override void OnLoad()
        {
            gameObject.SetActive(!isAcquired);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json.Add(nameof(isAcquired), isAcquired);
            return json;
        }

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isAcquired = true;
                UIManager.Instance.toastUI.Show("열쇠를 획득했다!");
                Core.ResourceManager.Instance.GiveMasterKeyToPlayer();
                gameObject.SetActive(false);
            }
        }

    }
}