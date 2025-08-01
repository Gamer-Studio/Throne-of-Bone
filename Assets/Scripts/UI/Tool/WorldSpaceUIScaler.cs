using ToB.Core;
using UnityEngine;

namespace ToB.UI
{
    public class WorldSpaceUIScaler : MonoBehaviour
    {
        [SerializeField] private Transform parentElement; 
        void LateUpdate()
        {
            Vector3 scaleUnit = Vector3.one;
            
            if (parentElement.localScale.x < 0) scaleUnit.x = -1;
            
            transform.localScale = scaleUnit * GameCameraManager.Instance.zoomRatio / 100;
        }
    }
}
