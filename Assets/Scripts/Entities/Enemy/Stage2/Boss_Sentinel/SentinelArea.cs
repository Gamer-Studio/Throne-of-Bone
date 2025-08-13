using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using ToB.IO;
using ToB.Core;
using ToB.Memories;
using ToB.Scenes.Stage;
using ToB.UI;
using ToB.Utils;
using ToB.Worlds;
using UnityEngine;
using UnityEngine.UI;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities
{
    public class SentinelArea : Room
    {
        private static readonly int CloneRise = Animator.StringToHash("CloneRise");
        private static readonly int DieContinue = Animator.StringToHash("DieContinue");
        [SerializeField] private Sentinel sentinel;
        [SerializeField] Location location;
        [SerializeField] private GameObject exitBlocker;
        [SerializeField] private GameObject bloodBubblePrefab;
        [SerializeField] private Transform leftBottomPos;

        [Header("클론 스폰")] 
        [field: SerializeField] Sentinel clone1;
        [field: SerializeField] Sentinel clone2;

        [Header("페이즈 배경")] 
        [SerializeField] private GameObject phase1BG;
        [SerializeField] private GameObject phase2BG;
        
        [Header("센티넬 말풍선")]
        [SerializeField] private GameObject speechBubbleRoot;
        [SerializeField] TextMeshProUGUI speechText;

        private bool visited;
        
        protected override void Awake()
        {
            base.Awake();
            location.OnPlayerEntered += PlayerEntered;
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            clone1.gameObject.SetActive(false);
            clone2.gameObject.SetActive(false);

            clone1.Agent.enabled = false;
            clone2.Agent.enabled = false;

            sentinel.gameObject.SetActive(false);
        }

        private void PlayerEntered()
        {
            if (visited) return;
            if(SAVE.Current != null && SAVE.Current.Achievements.KillSentinel) return;
            
            visited = true;
            StartCoroutine(SentinelRoomCoroutine());
        }
        #region BossSequence
        
        IEnumerator SentinelRoomCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            exitBlocker.SetActive(true);
            AudioManager.Stop(AudioType.Background);
            AudioManager.Play("3. Eclipsed Desolation",AudioType.Background);
            yield return new WaitForSeconds(0.5f);
            
            // TODO: 효수된 시체 연출
            
            yield return StartCoroutine(SentinelRise());
            
            // TODO: 대화
          
            sentinel.SetTarget(StageManager.Instance.player.transform);

            yield return new WaitUntil(() => sentinel.Phase == 2);
            yield return StartCoroutine(Phase2TransitionMoment());
            
            DebugSymbol.Get("LSH").Log("클리어");
            yield return new WaitUntil(()=> !sentinel.IsAlive);
            
            // 센티넬 처치 기록
            if(SAVE.Current != null) SAVE.Current.Achievements.KillSentinel = true;
            
            AudioManager.Stop(AudioType.Background);
            StageManager.Instance.ChangeGameState(GameState.CutScene);
            
            yield return StartCoroutine(SentinelDie());
            
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(UIManager.Instance.FadeOut(3));
            UIManager.Instance.fadePanel.color = new Color(0, 0, 0, 0);

            StageManager.Instance.EndGame();
        }

        IEnumerator Phase2TransitionMoment()
        {
            phase1BG.SetActive(false);
            phase2BG.SetActive(true);
            yield return null;
        }

        IEnumerator SentinelDie()
        {
            MemoriesManager.Instance.MemoryAcquired(2003);
            speechBubbleRoot.SetActive(true);
            speechText.color = Color.red;
            sentinel.dashParticle.Stop();

            yield return new WaitForSeconds(2);
            sentinel.Phase2Aura.SendEvent("OnStop");
            yield return StartCoroutine(TextCoroutine("…아직… 끝나지 않았어…"));
            yield return StartCoroutine(TextCoroutine("지켜야 해… 지켜야 해…"));
            sentinel.Animator.SetTrigger(EnemyAnimationString.Die);
            yield return StartCoroutine(TextCoroutine("그 분을… 그 분을… 반드시…"));
            yield return StartCoroutine(TextCoroutine("검을… 버릴 수 없어… 내 손으로…"));
            yield return StartCoroutine(TextCoroutine("내 손으로 지켜야 했는데…"));
            sentinel.Animator.SetTrigger(DieContinue);
            yield return StartCoroutine(TextCoroutine("…안 돼… 안 돼… 무너진다… 다 무너져…"));
            yield return StartCoroutine(TextCoroutine("막아야 해… 막아야…"));
            sentinel.Animator.SetTrigger(DieContinue);
            yield return StartCoroutine(TextCoroutine("으… 으아아아아악!!"));
            sentinel.GlowObject.SetActive(false);
            speechBubbleRoot.SetActive(false);
        }

        IEnumerator TextCoroutine(String text)
        {
            speechText.text = text;
            LayoutRebuilder.ForceRebuildLayoutImmediate(speechText.rectTransform);
            Canvas.ForceUpdateCanvases();

            yield return new WaitUntil(() => StageManager.Instance.cutSceneProcessCall);

            StageManager.Instance.cutSceneProcessCall = false;
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
            clone.prevHP = clone.Stat.CurrentHP;
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
            clone.Animator.SetTrigger(EnemyAnimationString.CloneDie);

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