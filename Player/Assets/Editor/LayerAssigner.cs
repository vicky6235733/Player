using UnityEditor;
using UnityEngine;

public class LayerAssigner : EditorWindow
{
    // 子物件的預設
    public GameObject childPrefab;

    [MenuItem("Tools/批量設置圖層子物件")]
    public static void ShowWindow()
    {
        GetWindow<LayerAssigner>("Batch Add Child Objects");
    }

    private void OnGUI()
    {
        // 輸入子物件的Prefab
        childPrefab = (GameObject)
            EditorGUILayout.ObjectField("Child Prefab", childPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Add Child to Selected Objects"))
        {
            AddChildToSelectedObjects();
        }
    }

    private void AddChildToSelectedObjects()
    {
        // 檢查是否選中物件
        if (childPrefab == null)
        {
            Debug.LogError("Please assign a child prefab.");
            return;
        }

        // 獲取選中的物件
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogError("No objects selected.");
            return;
        }

        foreach (GameObject selectedObject in selectedObjects)
        {
            // 為每個選中的物件創建一個新的子物件
            GameObject newChild = (GameObject)
                PrefabUtility.InstantiatePrefab(childPrefab, selectedObject.transform);
            newChild.transform.localPosition = Vector3.zero; // 設置相對於父物件的位置
            newChild.transform.localRotation = Quaternion.identity;
        }

        Debug.Log("Child objects added to selected objects.");
    }
}
