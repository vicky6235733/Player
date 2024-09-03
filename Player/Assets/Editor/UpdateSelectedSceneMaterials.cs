using UnityEngine;
using UnityEditor;

public class UpdateSelectedSceneMaterials : MonoBehaviour
{
    [MenuItem("Tools/批量指定資產材質")]
    static void AssignMaterialToSelectedAssets()
    {
        // 弹出文件选择窗口，选择材质文件
        string path = EditorUtility.OpenFilePanel("Select Material", "Assets/Materials", "mat");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("未选择任何材质文件");
            return;
        }

        // 转换为相对路径
        path = "Assets" + path.Substring(Application.dataPath.Length);
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);

        if (material == null)
        {
            Debug.LogError("无法加载材质: " + path);
            return;
        }

        // 获取选中的所有对象
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            // 确保对象是一个模型或Prefab
            string assetPath = AssetDatabase.GetAssetPath(obj);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (asset != null)
            {
                // 加载该资产的所有Renderer组件
                Renderer[] renderers = asset.GetComponentsInChildren<Renderer>(true);

                foreach (Renderer renderer in renderers)
                {
                    // 为资产中的所有Renderer组件指定材质
                    renderer.sharedMaterial = material;
                }

                // 保存更改
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
                Debug.Log("材质已分配给: " + asset.name);
            }
            else
            {
                Debug.LogWarning("选中的对象不是有效的资产: " + obj.name);
            }
        }
    }
}
