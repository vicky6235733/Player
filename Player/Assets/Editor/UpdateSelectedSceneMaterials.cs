using UnityEditor;
using UnityEngine;

public class UpdateSelectedSceneMaterials : EditorWindow
{
    [MenuItem("Tools/Update Selected Scene Materials(沒用處)")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<UpdateSelectedSceneMaterials>("Update Selected Scene Materials");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Update Selected Scene Materials"))
        {
            UpdateMaterialsForSelectedObjects();
        }
    }

    private static void UpdateMaterialsForSelectedObjects()
    {
        // 获取用户选择的所有对象
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "No objects selected. Please select some objects in the scene.", "OK");
            return;
        }

        foreach (GameObject obj in selectedObjects)
        {
            // 获取对象及其子物体上的所有MeshRenderer组件
            MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer renderer in renderers)
            {
                Material[] materials = renderer.sharedMaterials;

                if (materials.Length == 0)
                {
                    Debug.Log($"Object: {renderer.gameObject.name} has no materials.");
                }
                else
                {
                    // 强制刷新材质
                    renderer.materials = materials;

                    // 标记对象为已更改
                    EditorUtility.SetDirty(renderer.gameObject);

                    // 记录日志以验证更新
                    foreach (Material mat in materials)
                    {
                        Debug.Log($"Updated material: {mat.name} on object: {renderer.gameObject.name}");
                    }
                }
            }
        }

        // 刷新场景视图
        SceneView.RepaintAll();

        Debug.Log("Selected scene materials updated.");
    }
}
