using System.Linq;
using ToB.Worlds;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Season))]
public class SeasonDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);

    var selectedSeason = property.Get<Season>();

    var index = EditorGUI.Popup(EditorGUILayout.GetControlRect(), "계절", selectedSeason.Id,
      (from season in Season.ActiveSeasons select season.Name).ToArray());
    selectedSeason = Season.ActiveSeasons[index];
    property.Set(selectedSeason);

    EditorGUI.EndProperty();
  }
}