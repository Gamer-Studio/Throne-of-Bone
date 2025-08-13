using System;
using System.Collections;
using ToB.UI;
using UnityEngine;

namespace ToB.Core
{
    /// <summary>
    /// UI 매니저 휘하에 있습니다
    /// </summary>
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private GameObject manaCutSceneRayBlocker;

        private void Awake()
        {
            manaCutSceneRayBlocker.SetActive(false);
        }

        public IEnumerator ObtainedManaCutScene()
        {
            bool skillUp = false;

            // 여기서만 필요한 소모성 함수
            void SkillUp()
            {
                skillUp = true;
            }
            
            BattleSkillPanelController.OnSkillLearned += SkillUp;
            
            UIManager.Instance.mainBookUI.OpenSkillUI();
            manaCutSceneRayBlocker.SetActive(true);
            
            yield return new WaitUntil(() => skillUp);
            
            BattleSkillPanelController.OnSkillLearned -= SkillUp;
            UIManager.Instance.mainBookUI.CloseBook();
            manaCutSceneRayBlocker.SetActive(false);
            
        }
    }
}
