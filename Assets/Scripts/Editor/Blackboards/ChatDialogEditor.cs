using ToB.Blackboards;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatDialogEditorWindow : EditorWindow
{
    private ChatDialogGraph graph;
    private ChatDialogView graphView;
    
    [OnOpenAsset(1)] // 우선순위 낮은 숫자가 먼저 실행
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj is ChatDialogGraph graphAsset)
        {
            var wnd = GetWindow<ChatDialogEditorWindow>();
            wnd.titleContent = new GUIContent("Chat Dialog Board");
            wnd.LoadGraph(graphAsset);
            return true; // 처리 완료
        }
        return false; // 다른 처리기로 넘어감
    }
    
    private void LoadGraph(ChatDialogGraph asset)
    {
        graph = asset;
        RebuildGraphView();
    }

    private void RebuildGraphView()
    {
        // 혹시 기존 graphView 있으면 제거
        if (graphView != null)
            rootVisualElement.Remove(graphView);

        graphView = new ChatDialogView();
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);

        GenerateBlackboard();
    }

    private void ConstructGraphView()
    {
        graphView = new ChatDialogView
        {
            name = "My Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateBlackboard()
    {
        var blackboard = new Blackboard(graphView);
        blackboard.title = "Variables";

        // Example: Add a section
        var section = new BlackboardSection { title = "My Section" };
        blackboard.Add(section);

        // Example: Add a field
        var field = new BlackboardField { text = "myVariable", typeText = "float" };
        section.Add(field);

        // Handle rename callback
        blackboard.addItemRequested = (blk) =>
        {
            Debug.Log("Add Item Requested");
        };

        blackboard.editTextRequested = (blk, element, newName) =>
        {
            var fieldElement = element as BlackboardField;
            if (fieldElement != null)
            {
                Debug.Log($"Renaming {fieldElement.text} to {newName}");
                fieldElement.text = newName;
            }
        };

        // Position on the left
        blackboard.SetPosition(new Rect(10, 30, 200, 300));
        graphView.Add(blackboard);
    }
    
    private void OnEnable()
    {
        ConstructGraphView();
        GenerateBlackboard();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}

public class ChatDialogView : GraphView
{
    public ChatDialogView()
    {
        style.flexGrow = 1;

        // 기본 줌, 드래그 설정
        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
    }
}
