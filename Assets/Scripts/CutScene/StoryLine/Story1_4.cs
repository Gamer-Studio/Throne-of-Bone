using System.Collections;
using ToB.Entities;
using UnityEngine;

namespace ToB.CutScene
{
    public class Story1_4 : DisposableCutScene
    {
        protected override IEnumerator CutSceneUnique()
        {
            yield return StartCoroutine(GrimoireAppear());
            yield return StartCoroutine(Grimoire.Say("...멈추세요. 앞에 무언가 있습니다."));
            yield return StartCoroutine(Grimoire.Say("...마물이군요."));
            yield return StartCoroutine(Grimoire.Say("마력에 물들어... 인간이 아니게 되어버린 존재들."));
            yield return StartCoroutine(Grimoire.Say("가까이 오고 있군요."));
            yield return StartCoroutine(Grimoire.Say("피할 수 없다면... 맞서야 합니다."));
            yield return StartCoroutine(Grimoire.Say("당신의 몸이 기억하고 있을지도 모릅니다."));
            yield return StartCoroutine(Grimoire.Say("...싸우는 법을."));
            yield return StartCoroutine(GrimoireDisappear());
        }
    }
}
