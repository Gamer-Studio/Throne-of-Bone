using System.Collections;
using Cinemachine;
using ToB.Core;
using ToB.Core.InputManager;
using ToB.Scenes.Stage;
using ToB.UI;
using UnityEngine;

namespace ToB.CutScene
{
    public class SpecialCutScenes : DisposableCutScene
    {
        public IEnumerator ObtainedFirstManaCutScene()
        {
            StageManager.Instance.ChangeGameState(GameState.CutScene);
            yield return StartCoroutine(GrimoireAppear());

            yield return StartCoroutine(Grimoire.Say("이건 단순한 조각이 아닙니다."));
            yield return StartCoroutine(Grimoire.Say("짙은 마력이 응집되어… 결정화된 것."));
            yield return StartCoroutine(Grimoire.Say("보통이라면, 이런 마력결정은 매우 사용하기 까다로운 물질입니다만..."));
            yield return StartCoroutine(Grimoire.Say("하지만 저라면... 이걸 이용할 방법이 있습니다."));
            yield return StartCoroutine(Grimoire.Say("제가 간직하고 있는 것 중, <color=#FFD300>‘영웅’<color=#FFFFFF>의 <color=#FFD300>전투 경험과 관련된 기록<color=#FFFFFF>이 있습니다."));
            yield return StartCoroutine(Grimoire.Say("그리고 지금 손에 넣은 이 마력 결정을 통해..."));
            yield return StartCoroutine(Grimoire.Say("그 기억을 당신에게 전달해 줄 수 있을지도 모릅니다."));
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);

            TOBInputManager.Instance.blockUICancel = true;
            yield return StartCoroutine(UIManager.Instance.tutorialManager.ObtainedManaCutScene());
            StageManager.Instance.ChangeGameState(GameState.CutScene);  // 위 코루틴의 마지막에 책을 닫는데 닫을 때 게임스테이트 Play가 되기에 도로 상태 덮어쓰는 부분
            TOBInputManager.Instance.blockUICancel = false;
            
            Color c = Color.red;
            c.a = 0.5f;
            UIManager.Instance.fadePanel.color = c;
            
            yield return null;
            yield return StartCoroutine(UIManager.Instance.FadeIn(1f));
           
            c = Color.black;
            c.a = 0;
            UIManager.Instance.fadePanel.color = c;
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            
            yield return StartCoroutine(Grimoire.Say("......성공이군요."));
            yield return StartCoroutine(Grimoire.Say("당신의 몸이, 기억을 잘 받아들였습니다."));
            yield return StartCoroutine(Grimoire.Say("이제부터 마력 결정을 모을 때마다"));
            yield return StartCoroutine(Grimoire.Say("당신은 더 많은 ‘전투의 기억’을 얻게 될 겁니다."));
            yield return StartCoroutine(Grimoire.Say("그 기억들은... 이 세계에서 살아남기 위한 힘이 되어줄 테지요."));
            
            yield return StartCoroutine(GrimoireDisappear());
            StageManager.Instance.ChangeGameState(GameState.Play);
        }
        
        protected override IEnumerator CutSceneUnique()
        {
            yield return null;
        }
    }
}
