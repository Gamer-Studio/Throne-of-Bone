#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ToB.Editor.Unique
{
    [CustomEditor(typeof(Entities.Grimoire))]
    public class GrimoireEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var grimoire = (Entities.Grimoire)target;

            if (Application.isPlaying)
            {
                GUILayout.Space(10);

                if (GUILayout.Button("▶ Appear 연출 실행"))
                {
                    grimoire.Appear();
                }

                if (GUILayout.Button("⏹ Disappear 연출 실행"))
                {
                    grimoire.Disappear();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Play 모드에서만 연출 확인 가능합니다.", MessageType.Info);
            }
        }
    }
#endif
}