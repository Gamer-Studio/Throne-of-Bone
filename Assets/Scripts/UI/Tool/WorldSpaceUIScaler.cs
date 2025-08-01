using ToB.Core;
using UnityEngine;

namespace ToB.UI
{
    public class WorldSpaceUIScaler : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.localScale = Vector3.one * GameCameraManager.Instance.zoomRatio / 100;
        }
    }
}
