using System.Collections.Generic;
using System.Text.RegularExpressions;
using ToB.Entities.Skills;
using UnityEditor;
using UnityEngine;

namespace ToB.Editor
{
  
    public class SkillCSVImporter : EditorWindow
    {
        private TextAsset csvFile;
        private BattleSkillData skillDatabaseSO;

        [MenuItem("Tools/Import Skill CSV")]
        public static void ShowWindow()
        {
            GetWindow<SkillCSVImporter>("Import Skill CSV");
        }

        private void OnGUI()
        {
            csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
            skillDatabaseSO = (BattleSkillData)EditorGUILayout.ObjectField("Skill Database SO", skillDatabaseSO, typeof(BattleSkillData), false);

            if (GUILayout.Button("Import CSV to SkillDatabase"))
            {
                if (csvFile == null || skillDatabaseSO == null)
                {
                    Debug.LogError("CSV File과 Skill Database SO를 지정하세요.");
                    return;
                }

                ImportCSV(csvFile, skillDatabaseSO);
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

        private void ImportCSV(TextAsset csv, BattleSkillData database)
        {
            database.BattleSkillDataBase.Clear();

            string[] lines = csv.text.Split('\n');

            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var tokens = ParseCsvLine(line);
                if (tokens.Count < 12) continue;

                SkillData skill = new SkillData
                {
                    index = int.Parse(tokens[0]),
                    id = int.Parse(tokens[1]),
                    skillName = tokens[2],
                    tier = int.Parse(tokens[3]),
                    upStat1 = float.Parse(tokens[4]),
                    upStat2 = float.Parse(tokens[5]),
                    skillType = (SkillType)System.Enum.Parse(typeof(SkillType), tokens[6]),
                    goldCost = int.Parse(tokens[7]),
                    manaCost = int.Parse(tokens[8]),
                    reqID1 = int.Parse(tokens[9]),
                    reqID2 = int.Parse(tokens[10]),
                    description = tokens[11]
                };

                database.BattleSkillDataBase.Add(skill);
            }

            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            Debug.Log($"CSV Import 완료: {database.BattleSkillDataBase.Count}개 스킬 등록됨");
        }
    }
}
