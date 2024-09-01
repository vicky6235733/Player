using UnityEditor;
using UnityEngine;

public class BatchUnpackPrefab : EditorWindow
{
    [MenuItem("Tools/批量断开物件与资产连接")]
    public static void ShowWindow()
    {
        GetWindow<BatchUnpackPrefab>("Batch Unpack Prefabs");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Unpack Selected Prefabs"))
        {
            UnpackSelectedPrefabs();
        }
    }

    private void UnpackSelectedPrefabs()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected!");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(obj);

            // 只检查 Prefab 是否为 Connected 状态
            if (prefabInstanceStatus == PrefabInstanceStatus.Connected)
            {
                PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                Debug.Log($"{obj.name} has been unpacked.");
            }
            else
            {
                Debug.LogWarning($"{obj.name} is not a prefab instance.");
            }
        }

        Debug.Log("Batch unpacking completed.");
    }
}
