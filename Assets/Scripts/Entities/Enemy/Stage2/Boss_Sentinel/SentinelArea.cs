using System;
using System.Collections;
using DG.Tweening;
using ToB.Scenes.Stage;
using ToB.Utils;
using UnityEngine;

namespace ToB.Entities
{
    public class SentinelArea : MonoBehaviour
    {
        private static readonly int CloneRise = Animator.StringToHash("CloneRise");
        [SerializeField] private Sentinel sentinel;
        [SerializeField] Location location;
        [SerializeField] private GameObject exitBlocker;
        [SerializeField] private GameObject bloodBubblePrefab;
        [SerializeField] private Transform leftBottomPos;

        [Header("클론 스폰")] [field: SerializeField]
        Sentinel clone1;
        [field: SerializeField] Sentinel clone2;

        private void Awake()
        {
            location.OnPlayerEntered += PlayerEntered;
            location.OnPlayerExit += PlayerExit;
        }

        private void OnEnable()
        {
            clone1.gameObject.SetActive(false);
            clone2.gameObject.SetActive(false);

            clone1.Agent.enabled = false;
            clone2.Agent.enabled = false;

            sentinel.gameObject.SetActive(false);
        }

        private void PlayerEntered()
        {
            
            StartCoroutine(SentinelRoomCoroutine());
        }

        private void PlayerExit()
        {
            sentinel.SetTarget(null);
        }

        #region BossSequence
        IEnumerator SentinelRoomCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            exitBlocker.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            
            // TODO: 효수된 시체 연출
            
            yield return StartCoroutine(SentinelRise());
            
            // TODO: 대화
          
            sentinel.SetTarget(StageManager.Instance.player.transform);

            yield return new WaitUntil(() => sentinel.Phase == 2);
            
            yield return new WaitUntil(()=> !sentinel.IsAlive);
            yield return StartCoroutine(SentinelDie());
        }

        private string SentinelDie()
        {
            throw new NotImplementedException();
        }

        IEnumerator SentinelRise()
        {
            StageManager.Instance.ChangeGameState(GameState.UI);
            sentinel.gameObject.SetActive(true);
            sentinel.Animator.SetTrigger(CloneRise);
            
            yield return new WaitForSeconds(1.3f);
            StageManager.Instance.ChangeGameState(GameState.Play);
        }
        
        #endregion
        
        public void StartBloodBubbles()
        {
            StartCoroutine(BloodBubbles());
        }

        IEnumerator BloodBubbles()
        {
            yield return new WaitForSeconds(2f);
            for (int i = 1; i < 23; i += 4)
            {
                GameObject bubbleObj = bloodBubblePrefab.Pooling();
                bubbleObj.transform.position = leftBottomPos.position + new Vector3(i, 0, 0);
            }

            yield return new WaitForSeconds(1.8f);
            for (int i = 3; i < 23; i += 4)
            {
                GameObject bubbleObj = bloodBubblePrefab.Pooling();
                bubbleObj.transform.position = leftBottomPos.position + new Vector3(i, 0, 0);
            }

            yield return new WaitForSeconds(1.8f);
            for (int i = 1; i < 23; i += 4)
            {
                GameObject bubbleObj = bloodBubblePrefab.Pooling();
                bubbleObj.transform.position = leftBottomPos.position + new Vector3(i, 0, 0);
            }
        }

        public void SpawnClones()
        {
            StartCoroutine(InitClone(clone1));
            StartCoroutine(InitClone(clone2));
        }

        IEnumerator InitClone(Sentinel clone)
        {
            clone.gameObject.SetActive(true);
            clone.Animator.SetTrigger(CloneRise);
            yield return null;
            clone.Hitbox.enabled = false;
            clone.Agent.enabled = true;

            yield return new WaitForSeconds(1.3f);
            Debug.Log("히트박스 키기");
            clone.Hitbox.enabled = true;
            clone.SetTarget(StageManager.Instance.player.transform);
        }

        public void ClearClones()
        {
            KillClone(clone1);
            KillClone(clone2);
        }

        private void KillClone(Sentinel clone)
        {
            clone.Hitbox.enabled = false;
            clone.Agent.enabled = false;
            clone.SetTarget(null);
            clone.Animator.SetTrigger(EnemyAnimationString.Die);

            DOVirtual.DelayedCall(1.3f, () => { clone.gameObject.SetActive(false); });
        }

        public Vector3 GetRandomFloorPosition()
        {
            Vector3 pos = leftBottomPos.position;

            pos.x += UnityEngine.Random.Range(1, location.Width - 1);

            return pos;
        }
    }
}