using System;
using ToB.Core;
using UnityEngine;

namespace ToB.UI
{
    public class WorldSpaceUIScaler : MonoBehaviour
    {
        [SerializeField] private Transform parentElement;
        private Vector3 originalScale;
        private void Awake()
        {
            originalScale = transform.localScale;
        }

        void LateUpdate()
        {
            int scaleUnit = 1;
            
            if (parentElement.localScale.x < 0) scaleUnit = -1;
            else if (Mathf.Approximately(parentElement.eulerAngles.y, 180)) scaleUnit = -1;
            
            transform.localScale = new Vector3(originalScale.x * scaleUnit, originalScale.y, originalScale.z)  * GameCameraManager.Instance.zoomRatio;
            
            
            
            // Vector3 scaleUnit = Vector3.one;
            //
            // if (parentElement.localScale.x < 0) scaleUnit.x = -1;
            // else scaleUnit.x = 1;
            //
            // if (parentElement.eulerAngles.y < 0) scaleUnit.x = -1;
            // else scaleUnit.x = 1;
            //
            //transform.localScale = scaleUnit * GameCameraManager.Instance.zoomRatio / 100;
        }
    }
}
