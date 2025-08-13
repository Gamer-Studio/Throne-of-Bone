using System;
using System.IO;
using ToB.Core.InputManager;
using ToB.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ToB.Core.InputManager
{
    public class KeyBindManager : MonoBehaviour
    {
        [SerializeField] InputActionAsset inputActionAsset;
        
        const string KeyBindsFileName = "KeyBinds.json";
        private static string KeyBindsFilePath =>
            Path.Combine(Application.persistentDataPath, KeyBindsFileName);
        

        public void LoadKeySettings()
        {
            DebugSymbol.ETC.Log(KeyBindsFilePath);
            if (!File.Exists(KeyBindsFilePath))
            {
                DebugSymbol.ETC.Log("사전 키셋팅이 없어 기본값을 불러옵니다.");
                return;
            }
            DebugSymbol.ETC.Log("키셋팅 파일을 찾았습니다");
            string json = File.ReadAllText(KeyBindsFilePath);
            
            TOBInputManager.Instance.PlayerInput.actions.LoadBindingOverridesFromJson(json);
        }

        public void SaveCurrentKeySettings()
        {
            string json = inputActionAsset.SaveBindingOverridesAsJson();
            SaveToJsonFile(json);   // 공용 세이브 로직 같은 거 다뤄주는 스크립트 있으면 이 메서드 다뤄주시면 될 것 같습니다
        }

        private void SaveToJsonFile(string json)
        {
            DebugSymbol.ETC.Log("키셋팅을 저장합니다");
            File.WriteAllText(KeyBindsFilePath, json);
        }
    }
}
