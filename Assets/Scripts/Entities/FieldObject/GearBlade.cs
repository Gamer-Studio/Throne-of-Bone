using System.Collections;
using ToB.Entities.Interface;
using ToB.Worlds;
using UnityEngine;

namespace ToB.Entities.FieldObject
{
    public class GearBlade : FieldObjectProgress, IAttacker
    {
        private readonly int IsActivated = Animator.StringToHash("IsActivated");

        public enum BladeMode
        {
            Always,
            Timed,
            Disabled
        }

        [SerializeField] private Animator animator;
        [SerializeField] public BladeMode bladeMode;
        [SerializeField] public float knockBackPower;
        [SerializeField] public float damage;
        [SerializeField] public float activeTime;
        [SerializeField] public float safeTime;
        private bool isActivated;
        private Coroutine C_SpinningTimer;

        private void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            GearBladeModeToggle(bladeMode);
        }

        private void SpinningCoroutine()
        {
            if (C_SpinningTimer != null)
            {
                StopCoroutine(C_SpinningTimer);
                C_SpinningTimer = null;
            }

            C_SpinningTimer = StartCoroutine(Spinning());
        }

        private IEnumerator Spinning()
        {
            while (bladeMode == BladeMode.Timed)
            {
                isActivated = true;
                animator.SetBool(IsActivated, true);
                yield return new WaitForSeconds(activeTime);

                isActivated = false;
                animator.SetBool(IsActivated, false);
                yield return new WaitForSeconds(safeTime);
            }

            isActivated = false;
            animator.SetBool(IsActivated, false);
            C_SpinningTimer = null;
        }

        /// <summary>
        /// 혹시나 레버 등으로 기어의 작동을 꺼야 할 일이 있을 때, 모드 변환을 위해 호출.
        /// Always는 항상 돌고, Timed는 돌다가 멈췄다가 하며, Disable은 회전이 꺼짐
        /// </summary>
        /// <param name="_bladeMode"></param>
        public void GearBladeModeToggle(BladeMode _bladeMode)
        {
            bladeMode = _bladeMode;

            if (_bladeMode == BladeMode.Always)
            {
                if (C_SpinningTimer != null) StopCoroutine(C_SpinningTimer);
                C_SpinningTimer = null;
                isActivated = true;
                animator.SetBool(IsActivated, true);
            }
            else if (_bladeMode == BladeMode.Timed)
            {
                SpinningCoroutine();
            }

            else if (_bladeMode == BladeMode.Disabled)
            {
                if (C_SpinningTimer != null) StopCoroutine(C_SpinningTimer);
                C_SpinningTimer = null;
                isActivated = false;
                animator.SetBool(IsActivated, false);
            }

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && isActivated)
            {
                other.Damage(damage, this);
                other.KnockBack(knockBackPower, gameObject);
            }
        }

        [field: SerializeField] public bool Blockable { get; set; }
        [field: SerializeField] public bool Effectable { get; set; }
        public Vector3 Position => transform.position;
        public Team Team => Team.Enemy;
    }
}