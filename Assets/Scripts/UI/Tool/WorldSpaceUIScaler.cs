using System;
using ToB.Core;
using UnityEngine;

namespace ToB.UI
{
    public class WorldSpaceUIScaler : MonoBehaviour
    {
        [SerializeField] private Transform parentElement;
        private Vector3 originalScale;
        private Vector3 prevScale;

        public event Action ScaleChangedAction;
        private void Awake()
        {
            originalScale = transform.localScale;
            prevScale = originalScale;
        }

        void LateUpdate()
        {
            int scaleUnit = 1;
            
            if (parentElement.localScale.x < 0) scaleUnit = -1;
            else if (Mathf.Approximately(parentElement.eulerAngles.y, 180)) scaleUnit = -1;

            transform.localScale = new Vector3(originalScale.x * scaleUnit, originalScale.y, originalScale.z)  * GameCameraManager.Instance.zoomRatio;
            if (prevScale != transform.localScale)
            {
                prevScale = transform.localScale;
                ScaleChangedAction?.Invoke();
            }
        }
    }
}
