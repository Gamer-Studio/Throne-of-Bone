using ToB.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public static class AddressableAssetImporter
{
  public const string InitAudioAssetName = "Assets/Asset Initializer/Init Audio Asset";
  
  [MenuItem(InitAudioAssetName, true)]
  private static bool ValidateInitAudioAsset()
  {
    foreach (var obj in Selection.objects)
    {
      if(obj is AudioClip)
        return true;
    }
    
    return false;
  }

  /// <summary>
  /// 오디오 에셋의 어드레서블 주소 설정 자동화
  /// </summary>
  [MenuItem(InitAudioAssetName)]
  private static void InitAudioAsset()
  {
    // 어드레서블 설정 찾기
    var settings = AddressableAssetSettingsDefaultObject.Settings;
    if (settings == null)
    {
      Debug.LogError("AddressableAssetSettings 파일이 존재하지 않습니다. Addressables이 설정되지 않았을 수 있습니다.");
      return;
    }
    
    foreach (var obj in Selection.objects)
    {
      if (obj is not AudioClip) continue;
      
      string path = AssetDatabase.GetAssetPath(obj),
        guid = AssetDatabase.AssetPathToGUID(path);
      var entry = settings.FindAssetEntry(guid);

      if (entry == null)
      {
        entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
        entry.SetAddress($"{AudioManager.Label}/{obj.name}");
      }
      else
      {
        entry.SetAddress($"{AudioManager.Label}/{obj.name}");
      }
    }
    
    AssetDatabase.SaveAssets();
    EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
  }
}
