using ToB.Core;
using ToB.Entities.FieldObject;
using UnityEngine;

namespace ToB.Entities
{
    public class testResourceGetter : MonoBehaviour, IInteractable
    {
        [SerializeField] public int amount;
        [SerializeField] public InfiniteResourceType type;

        private void Awake()
        {
            IsInteractable = true;
        }

        public void Interact()
        {
            Debug.Log("interact");
            Core.ResourceManager.Instance.SpawnResources(type,amount,transform);
        }

        public bool IsInteractable { get; set; }
    }
}