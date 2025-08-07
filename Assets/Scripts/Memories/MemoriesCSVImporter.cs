using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ToB.Memories
{
    public class MemoriesCSVImporter : EditorWindow
    {
        private TextAsset csvFile;
        private MemoriesDataSO memoriesDataBase;
        
        [MenuItem("Tools/Import Memories CSV")]
        public static void ShowWindows()
        {
            GetWindow<MemoriesCSVImporter>("Import Memories CSV");
        }
        
        private void OnGUI()
        {
            csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
            memoriesDataBase = (MemoriesDataSO)EditorGUILayout.ObjectField("Memories Database SO", memoriesDataBase, typeof(MemoriesDataSO), false);

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
        
        private List<string> ParseCsvLine(string line)
        {
            var matches = Regex.Matches(line, @"(?:^|,)(?:""(.*?)""|([^"",]*))");
            var values = new List<string>();

            foreach (Match match in matches)
            {
                values.Add(match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value);
            }

            return values;
        }

        private void ImportCSV(TextAsset csv, MemoriesDataSO database)
        {
            database.memoriesDataBase.Clear();
            
            string[] lines = csv.text.Split('\n');

            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var tokens = ParseCsvLine(line);
                if (tokens.Count < 4) continue;

                Memories memory = new Memories
                {
                    id = int.Parse(tokens[0]),
                    name = tokens[1],
                    description = tokens[2],
                    relatedIconFileName = tokens[3],
                    //relatedIcon = Addressables.LoadAssetAsync<Sprite>($"MemoriesImages/{memories.relatedIconFileName}")
                };
                
                database.memoriesDataBase.Add(memory);
            }
            
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"CSV Import 완료 : {database.memoriesDataBase.Count}개 일지 등록됨");
        }
    }
}