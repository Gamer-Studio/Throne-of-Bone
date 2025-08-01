using ToB.Player;
using ToB.Scenes.Stage;
using UnityEditor;
using UnityEngine;

namespace ToB.Editor
{
  public class StageDebugger : EditorWindow
  {
    [MenuItem("Debug/Stage Debugger")]
    private static void ShowWindow()
    {
      var window = GetWindow<StageDebugger>();
      window.titleContent = new GUIContent("Stage Debugger");
      window.Show();
    }

    private int stageIndex = 0, roomIndex = 0;
    private Vector3 targetPosition = Vector3.zero;

    private void OnGUI()
    {
      if (StageManager.HasInstance)
      {
        var controller = StageManager.RoomController;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("현재 스테이지 Index");
        GUILayout.BeginVertical("helpbox");
        EditorGUILayout.LabelField(controller.currentRoom.stageIndex.ToString());
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("현재 방 Index");
        GUILayout.BeginVertical("helpbox");
        EditorGUILayout.LabelField(controller.currentRoom.roomIndex.ToString());
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("이동할 스테이지 Index");
        stageIndex = EditorGUILayout.IntField(stageIndex);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("이동할 방 Index");
        roomIndex = EditorGUILayout.IntField(roomIndex);
        EditorGUILayout.EndHorizontal();
        
        targetPosition = EditorGUILayout.Vector3Field("이동할 방 내부 위치", targetPosition);

        if (GUILayout.Button("플레이어 이동"))
        {
          foreach (var pair in controller.loadedRooms)
            if(pair.Value) Destroy(pair.Value.gameObject);
          
          controller.loadedRooms.Clear();
          
          var room = controller.LoadRoom(stageIndex, roomIndex, true);
          
          PlayerCharacter.Instance.transform.position = room.transform.position + targetPosition;
        }
        
        EditorGUILayout.Space();
        PlayerCharacter.Instance.invincibility = EditorGUILayout.Toggle("플레이어 무적 설정", PlayerCharacter.Instance.invincibility);
      }
      else
      {
        EditorGUILayout.LabelField("현재 스테이지가 로딩되지 않았습니다.");
      }
    }
  }
}