using System.Collections;
using Cinemachine;
using DG.Tweening;
using ToB.Core;
using ToB.IO;
using ToB.Player;
using ToB.Scenes.Stage;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AudioType = ToB.Core.AudioType;

namespace ToB.Entities
{
    public class InGameFirstCutScene : MonoBehaviour
    {
        Location location;
        [SerializeField]private DepthOfField blur;
        CinemachineVirtualCamera CurrentCamera => GameCameraManager.Instance.CurrentCamera;
        private PlayerCharacter Player => StageManager.Instance.player;
        private Grimoire Grimoire => StageManager.Instance.player.grimoire;
        
        bool isFirstEnter = true;
        private void Awake()
        {
            if (!Camera.main) return;
            Volume volume = Camera.main.GetComponent<Volume>();
            volume.profile.TryGet(out blur);
            location = GetComponent<Location>();
            
        }

        public void StartCutScene()
        {
            if (!isFirstEnter) return;
            isFirstEnter = false;
            StartCoroutine(Sequence());
        }
        
        IEnumerator Sequence()
        {
            if (SAVE.Current == null) yield break;
            if (!SAVE.Current.isFirstEnter)
            {
                yield break;
            }

            StageManager.Instance.ChangeGameState(GameState.CutScene);
            blur.active = true;
            
            yield return new WaitForSeconds(1f);
            Tween blurTween = DOTween.To(() => blur.aperture.value, x => blur.aperture.value = x, 32, 3f);
            //yield return blurTween.WaitForCompletion();
            yield return new WaitForSeconds(1.8f);
            
            float currentZoomSize = CurrentCamera.m_Lens.OrthographicSize;  // 시퀀스 끝나고 돌려놓기 위해 킵
            yield return StartCoroutine(GameCameraManager.Instance.Zoom(3f, 1f));
            
            Grimoire.Appear();
            yield return new WaitForSeconds(Grimoire.MotionTime);

            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            yield return null;
            AudioManager.Stop(AudioType.Background);
            AudioManager.Play("1.Stage", AudioType.Background);
            yield return StartCoroutine(Grimoire.Say("...일어났군요."));
            yield return StartCoroutine(Grimoire.Say("..."));
            yield return StartCoroutine(Grimoire.Say("...안심하세요. 저는 당신의 적이 아닙니다."));
            yield return StartCoroutine(Grimoire.Say("보다시피 저는 마도서."));
            yield return StartCoroutine(Grimoire.Say("어떤 영웅의 영혼이 깃든... 오래된 고서입니다."));
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);
            
            
            yield return new WaitForSeconds(1);
            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            yield return StartCoroutine(Grimoire.Say("혹시… 어떤 기억이라도 떠오르는 게 있습니까?"));
            yield return StartCoroutine(Grimoire.Say("........"));
            yield return StartCoroutine(Grimoire.Say("...기억이, 없으시군요."));
            yield return StartCoroutine(Grimoire.Say("그렇다면, 더욱 서로 도움이 필요하겠군요."));
            yield return StartCoroutine(Grimoire.Say("자세한 이야기는 나중에 드리겠습니다."));
            yield return StartCoroutine(Grimoire.Say("지금 이곳은, 그리 안전한 장소가 아니니까요."));
            
            yield return StartCoroutine(Grimoire.Say("여긴… 오랜 세월 동안 마력에 잠식되어 마물만이 가득한 폐허."));
            yield return StartCoroutine(Grimoire.Say("변이된 마물들이 언제 나타날지 모릅니다."));
            yield return StartCoroutine(Grimoire.Say("지금은… 서둘러 움직여야 할 때입니다."));
            yield return StartCoroutine(Grimoire.Say("기억을 잃은 지금은 혼란스럽겠지만…"));
            yield return StartCoroutine(Grimoire.Say("당신이 이곳에 깨어난 ‘의미’는, 분명히 있을 겁니다."));
            yield return StartCoroutine(Grimoire.Say("괜찮습니다. 제가 도와드릴게요."));
            yield return StartCoroutine(Grimoire.Say("기억을 되찾고, 이곳을 빠져나갈 길을… 함께 찾아봅시다."));
            
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);
            
            Grimoire.Disappear();
            yield return new WaitForSeconds(Grimoire.MotionTime);
            yield return StartCoroutine(GameCameraManager.Instance.Zoom(currentZoomSize, 1f));
            
            StageManager.Save();
            StageManager.Instance.ChangeGameState(GameState.Play);
        }
        

    }
}
