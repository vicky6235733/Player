using UnityEditor;
using UnityEngine;

public class BatchAssignMaterial : EditorWindow
{
    private Material selectedMaterial;

    [MenuItem("Tools/批量指定模型材質(用於資產)")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<BatchAssignMaterial>("Batch Assign Material to Models");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Material to Selected Models", EditorStyles.boldLabel);

        // 按钮选择材质
        if (GUILayout.Button("Select Material"))
        {
            string materialPath = EditorUtility.OpenFilePanel("Select Material", "Assets", "mat");
            if (!string.IsNullOrEmpty(materialPath))
            {
                string relativePath = "Assets" + materialPath.Substring(Application.dataPath.Length);
                selectedMaterial = AssetDatabase.LoadAssetAtPath<Material>(relativePath);
            }
        }

        if (selectedMaterial != null)
        {
            GUILayout.Label("Selected Material: " + selectedMaterial.name, EditorStyles.label);
        }

        if (GUILayout.Button("Assign Material to Selected Models"))
        {
            if (selectedMaterial != null)
            {
                AssignMaterialToSelectedModels();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "No material selected. Please select a material first.", "OK");
            }
        }
    }

    private void AssignMaterialToSelectedModels()
    {
        // 获取用户选择的所有对象
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (model != null)
            {
                // 获取模型上的所有MeshRenderer组件
                MeshRenderer[] renderers = model.GetComponentsInChildren<MeshRenderer>();

                foreach (MeshRenderer renderer in renderers)
                {
                    // 将选定的材质分配给模型的所有材质槽位
                    renderer.sharedMaterial = selectedMaterial;
                }

                Debug.Log($"Assigned material to model: {path}");
            }
            else
            {
                Debug.LogWarning($"Selected object is not a model: {path}");
            }
        }

        // 刷新资源数据库
        AssetDatabase.Refresh();
        Debug.Log("Batch material assignment completed.");
    }
}
