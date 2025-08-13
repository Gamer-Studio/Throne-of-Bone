using System.Collections;
using ToB.Core;
using ToB.Entities;
using ToB.UI;
using UnityEngine;

namespace ToB.CutScene
{
    public class Story1_4 : DisposableCutScene
    {
        protected override IEnumerator CutSceneUnique()
        {
            yield return StartCoroutine(GrimoireAppear());
            yield return StartCoroutine(Grimoire.Say("......여기는, 괜찮겠군요."));
            yield return StartCoroutine(Grimoire.Say("이곳이라면 안전할 것 같습니다."));
            yield return StartCoroutine(Grimoire.Say("한동안은... 쉴 수 있을 겁니다."));

            yield return StartCoroutine(UIManager.Instance.FadeOut(2f));
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);
            
            yield return StartCoroutine(UIManager.Instance.FadeIn(2f));
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            
            yield return StartCoroutine(Grimoire.Say("......조금은, 익숙해지셨나요? 이 끝없는 어둠과 침묵에."));
            yield return StartCoroutine(Grimoire.Say("기억 없이 깨어난 당신에겐"));
            yield return StartCoroutine(Grimoire.Say("모든 게 낯설고 불안할 수밖에 없겠지요."));
            yield return StartCoroutine(Grimoire.Say("이곳은, 아주 오래전엔 찬란했던 왕국의 땅이었습니다."));
            yield return StartCoroutine(Grimoire.Say("하지만 지금은... 모종의 이유로 멸망하고"));
            yield return StartCoroutine(Grimoire.Say("폐허만이 남은 황량한 잔재일 뿐이죠."));
            yield return StartCoroutine(Grimoire.Say("정확히 무슨 일이 있었는지는, 저도 알지 못합니다."));
            yield return StartCoroutine(Grimoire.Say("당신과 제가 이렇게 연결된 이유 역시..."));
            yield return StartCoroutine(Grimoire.Say("지금으로선 설명할 수 없습니다."));
            yield return StartCoroutine(Grimoire.Say("왕국이 멸망하던 날..."));
            yield return StartCoroutine(Grimoire.Say("강대한 마력의 폭발 속에서 제 일부는 손실되었고"));
            yield return StartCoroutine(Grimoire.Say("그 후 저는 아주 오랜 시간"));
            yield return StartCoroutine(Grimoire.Say("이 땅 깊은 곳에 봉인되어 있었습니다."));
            yield return StartCoroutine(Grimoire.Say("기억도, 능력도... 페이지가 찢긴 책처럼 군데군데 비어있는 상태지요."));
            yield return StartCoroutine(Grimoire.Say("당신도, 저도... 지금의 이 세계가 어떤 모습인지"));
            yield return StartCoroutine(Grimoire.Say("완전히 이해하고 있는 건 아닙니다."));
            yield return StartCoroutine(Grimoire.Say("그러니 함께 알아가 보도록 하죠."));
            yield return StartCoroutine(Grimoire.Say("서로의 조각을, 하나씩 맞춰가면서요."));
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(false);
            
            yield return new WaitForSeconds(2f);
            
            Grimoire.SpeechBubble.ActiveBubbleRoot(true);
            
            
            yield return StartCoroutine(Grimoire.Say("......충분히 쉬셨다면..."));
            yield return StartCoroutine(Grimoire.Say("이제 다시, 앞으로 나아가죠."));
            
            yield return StartCoroutine(GameCameraManager.Instance.Zoom(savedZoomSize,2f));
            
            yield return StartCoroutine(Grimoire.Say("......그러고 보니"));
            yield return StartCoroutine(Grimoire.Say("제가 페이지를 잃은 건... 이미 수백 년 전의 일이군요."));
            yield return StartCoroutine(Grimoire.Say("마도서의 본체에서 뜯겨 나가, 마력을 잃은 채 흩어진 그 조각들이..."));
            yield return StartCoroutine(Grimoire.Say("지금까지 남아 있을 거라 기대하긴 어렵겠죠."));
            yield return StartCoroutine(Grimoire.Say("하지만 이곳은, 마력이 소용돌이치는 마경 그 자체."));
            yield return StartCoroutine(Grimoire.Say("잃어버린 페이지를 대신할 무언가를 찾을 수 있을지도 모르겠습니다."));
            yield return StartCoroutine(Grimoire.Say("그걸 찾게 된다면..."));
            yield return StartCoroutine(Grimoire.Say("마도서로서, 당신을 좀 더 잘 보조해드릴 수 있을 겁니다. "));
            yield return StartCoroutine(GrimoireDisappear());
        }
    }
}
