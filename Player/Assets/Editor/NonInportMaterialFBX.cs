using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class NonInportMaterialFBX : EditorWindow
{
    [MenuItem("Tools/批量設置不導入模型材質")]
    //批量禁用導入材質
    public static void ShowWindow()
    {
        GetWindow<NonInportMaterialFBX>("Batch Process Selected Models");
    }

    private void OnGUI()
    {
        // 当点击按钮时，执行批量设置
        if (GUILayout.Button("Set Selected FBX Materials to Not Import"))
        {
            SetSelectedMaterialsToNone();
        }
    }

    private static void SetSelectedMaterialsToNone()
    {
        // 获取用户选择的所有对象
        Object[] selectedObjects = Selection.objects;
        
        foreach (Object obj in selectedObjects)
        {
            // 获取FBX文件的路径
            string path = AssetDatabase.GetAssetPath(obj);
            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;

            if (modelImporter != null)
            {
                // 设置材质导入模式为不导入
                modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
                AssetDatabase.WriteImportSettingsIfDirty(path);

                Debug.Log($"Set materials to not import for: {path}");
            }
            else
            {
                Debug.LogWarning($"Selected object is not a model: {path}");
            }
        }
        
        // 重新导入所有已修改的资源
        AssetDatabase.Refresh();
        Debug.Log("Batch process completed.");
    }
}