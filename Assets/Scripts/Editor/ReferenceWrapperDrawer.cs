using ToB.Utils;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ReferenceWrapper))]
public class ReferenceWrapperDrawer : PropertyDrawer
{
  private ReferenceWrapper target = null;
  
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    target ??= property.Get<ReferenceWrapper>();
    
    var assetRefProperty = property.FindPropertyRelative(nameof(target.assetReference));
    
    EditorGUILayout.LabelField(label);
    
    {
      EditorGUI.indentLevel++;
      GUILayout.BeginVertical("helpbox");

      EditorGUILayout.PropertyField(assetRefProperty, new GUIContent("Asset Reference"));

      {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Asset GUID");
        target.path = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(target.assetReference.AssetGUID)
          .address;
        EditorGUILayout.TextField(target.path);
        EditorGUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
      EditorGUI.indentLevel--;
    }

    EditorGUI.EndProperty();
  }
}
