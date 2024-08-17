using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[System.Serializable]
public class PositionData
{
    public string ObjectName;  // 改為 string，因為 GameObject 無法直接序列化為 JSON
    public Vector3 Position;
    public Quaternion Rotation;
}

[System.Serializable]
public class PositionDataList
{
    public List<PositionData> positions = new List<PositionData>();
}

public class PositionSaver : EditorWindow
{
    private string fileName = "positionData.json";
    private List<GameObject> objectsToSave = new List<GameObject>();
    private List<string> draggedObjectNames = new List<string>(); // 用於顯示拖曳物件名稱

    [MenuItem("Tools/JSON批量儲存物件位置與角度")]
    public static void ShowWindow()
    {
        GetWindow<PositionSaver>("Save Objects' Position");
    }

    void OnEnable()
    {
        // 在窗口啟用時初始化拖曳物件名稱列表
        draggedObjectNames.Clear();
    }

    void OnGUI()
    {
        // 顯示拖曳區域
        DrawDragAndDropArea();

        // 顯示拖曳的物件名稱
        DisplayDraggedObjectNames();

        // 為文件名輸入框和保存按鈕設置位置
        Rect fileNameRect = new Rect(10, position.height - 60, position.width - 20, 20);
        Rect saveButtonRect = new Rect(10, position.height - 30, position.width - 20, 30);

        // 顯示文件名輸入框
        fileName = EditorGUI.TextField(fileNameRect, "File Name", fileName);

        // 顯示保存按鈕
        if (GUI.Button(saveButtonRect, "Save Positions"))
        {
            SavePositions();
        }
    }

    void DrawDragAndDropArea()
    {
        // 設置拖曳區域的位置和大小
        Rect dropArea = new Rect(10, 10, position.width - 20, position.height - 100); // 保留底部空間
        GUI.Box(dropArea, "Drag and Drop Objects Here");

        Event e = Event.current;

        if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
        {
            if (dropArea.Contains(e.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    ProcessDragAndDrop();
                    Repaint(); // 重新繪製窗口以更新顯示
                }
            }
        }
    }

    void ProcessDragAndDrop()
    {
        foreach (Object draggedObject in DragAndDrop.objectReferences)
        {
            GameObject go = draggedObject as GameObject;
            if (go != null && !objectsToSave.Contains(go))
            {
                objectsToSave.Add(go);
                draggedObjectNames.Add(go.name); // 添加物件名稱到列表中
            }
        }
    }

    void DisplayDraggedObjectNames()
    {
        // 設置顯示區域的位置和大小
        Rect displayArea = new Rect(10, 10, position.width - 20, position.height - 100); // 保留底部空間

        if (draggedObjectNames.Count > 0)
        {
            // 設置字體樣式
            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                padding = new RectOffset(10, 10, 0, 0),
                fontSize = 12
            };

            // 設置顯示內容的字串
            string names = string.Join("\n", draggedObjectNames);

            // 設置顯示區域的位置和大小
            Rect labelRect = new Rect(displayArea.x + 10, displayArea.y + 10, displayArea.width - 20, displayArea.height - 20);
            GUI.Label(labelRect, "Dragged Objects:\n" + names, style);
        }
    }

    void SavePositions()
    {
        PositionDataList positionDataList = new PositionDataList();

        foreach (GameObject obj in objectsToSave)
        {
            PositionData data = new PositionData
            {
                ObjectName = obj.name,
                Position = obj.transform.position,
                Rotation = obj.transform.rotation
            };
            positionDataList.positions.Add(data);
        }

        string json = JsonUtility.ToJson(positionDataList, true);

        // 打開文件選擇對話框
        string path = EditorUtility.SaveFilePanel("Save Position Data", "", fileName, "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, json);
            Debug.Log($"Position data saved to {path}");
        }
    }
}
