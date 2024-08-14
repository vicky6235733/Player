using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ExtractMaterialsFromFBX : MonoBehaviour
{
    [MenuItem("Tools/Extract Materials from Selected FBX")]
    //批量解壓縮材質，沒有用處
    static void ExtractMaterials()
    {
        string defaultDirectory = "Assets/Materials";
        string materialDirectory = EditorUtility.OpenFolderPanel("Select Folder to Extract Materials", defaultDirectory, "");
        string relativePath="";

        if(!string.IsNullOrEmpty(materialDirectory))
        {
            relativePath = "Assets" + materialDirectory.Replace(Application.dataPath, "");
        }else
        {
            Debug.LogWarning("Material extraction canceled or failed.");
            return;
        }

        foreach (Object obj in Selection.objects)
        {
            if (obj is GameObject)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

                if (modelImporter != null)
                {
                    modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
                    AssetDatabase.WriteImportSettingsIfDirty(relativePath);
                    AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
                    Debug.Log("Materials extracted to: " + relativePath);
                }
            }
        }
        AssetDatabase.Refresh();
    }
}