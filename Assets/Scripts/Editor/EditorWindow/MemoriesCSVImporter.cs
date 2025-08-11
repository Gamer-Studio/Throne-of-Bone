using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToB.Memories;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MemoriesCSVImporter : EditorWindow
{
  private TextAsset csvFile;
  private MemoriesDataSO memoriesDataBase;

  private void OnGUI()
  {
    csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
    memoriesDataBase =
      (MemoriesDataSO)EditorGUILayout.ObjectField("Memories Database SO", memoriesDataBase, typeof(MemoriesDataSO),
        false);

    if (GUILayout.Button("Import CSV to MemoriesDataBase"))
    {
      if (csvFile == null || memoriesDataBase == null)
      {
        Debug.LogError("CSV 파일과 Memories Database SO를 지정해 주세요.");
        return;
      }

      ImportCSV(csvFile, memoriesDataBase);
    }
  }


  [MenuItem("Tools/Import Memories CSV")]
  public static void ShowWindow()
  {
    GetWindow<MemoriesCSVImporter>("Import Memories CSV");
  }

  private List<string> ParseCsvLine(string line)
  {
    var matches = Regex.Matches(line, @"(?:^|,)(?:""(.*?)""|([^"",]*))");
    var values = new List<string>();

    foreach (Match match in matches)
      values.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);

    return values;
  }

  private void ImportCSV(TextAsset csv, MemoriesDataSO database)
  {
    if (database != null) database.memoriesDataBase.Clear();

    var lines = csv.text.Split('\n');

    for (var i = 2; i < lines.Length; i++)
    {
      var line = lines[i].Trim();
      if (string.IsNullOrEmpty(line)) continue;

      var tokens = ParseCsvLine(line);
      if (tokens.Count < 4) continue;

      var memories = new Memories
      {
        id = int.Parse(tokens[0]),
        name = tokens[1],
        description = tokens[2],
        relatedIconFileName = tokens[3]
      };
      memories.relatedIcon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Artwork/MemoriesSprite/{memories.relatedIconFileName}.png");
      database.memoriesDataBase.Add(memories);
    }

    EditorUtility.SetDirty(database);
    AssetDatabase.SaveAssets();
    Debug.Log($"CSV Import 완료 : {database.memoriesDataBase.Count}개 일지 등록됨");
  }
}