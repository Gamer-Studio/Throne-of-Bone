using System;
using DG.Tweening;
using ToB.Scenes.Stage;
using UnityEngine;

namespace ToB.Entities
{
    public class SentinelArea : MonoBehaviour
    {
        [SerializeField] private Sentinel sentinel;
        [SerializeField] Location location;
        [SerializeField] private GameObject exitBlocker;

        private void Awake()
        {
            location.OnPlayerEntered += PlayerEntered;
            location.OnPlayerExit += PlayerExit;
        }

        private void PlayerEntered()
        {
            sentinel.SetTarget(StageManager.Instance.player.transform);
            DOVirtual.DelayedCall(1f, () => exitBlocker.SetActive(true));
        }
        private void PlayerExit()
        {
            sentinel.SetTarget(null);
        }
    }
}
