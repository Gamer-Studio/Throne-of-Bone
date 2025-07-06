using System.Linq;
using ToB.Utils;
using UnityEditor;
using UnityEngine;

namespace ToB.Editor
{
  public class DebugSymbolEditor : EditorWindow
  {
    [MenuItem("Debug/Symbol Editor")]
    private static void ShowWindow()
    {
      var window = GetWindow<DebugSymbolEditor>();
      window.titleContent = new GUIContent("Symbol Editor");
      window.Show();
    }

    private string tempText;
    
    private void OnGUI()
    {
      var symbolList = DebugSymbol.Symbols.ToArray();

      foreach (var symbol in symbolList)
      {
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField(symbol.name);
        symbol.isActive = EditorGUILayout.Toggle(symbol);

        if (!symbol.dontDestroy && GUILayout.Button("X")) symbol.Release();
        
        EditorGUILayout.EndHorizontal();
      }
      
      tempText = EditorGUILayout.TextField(tempText);
      if (GUILayout.Button("디버그 심볼 추가"))
      {
        DebugSymbol.Get(tempText, true, false);
      }

    }
  }
}