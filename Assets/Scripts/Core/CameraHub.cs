using Cinemachine;
using ToB.Utils.Singletons;
using UnityEngine;

namespace ToB.Core
{
    public class CameraHub : Singleton<CameraHub>
    {
        [field:SerializeField] public Camera MainCamera { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera MainVirtualCamera { get; private set; }
    }
}
