using UnityEngine;

namespace ToB.Entities
{
    public class SavePosition : MonoBehaviour
    {
        public GameObject target;
        public Vector3 position;

        public void Save()
        {
            position = target.transform.position;
        }
    }
}
