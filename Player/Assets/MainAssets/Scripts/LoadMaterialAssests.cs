using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadMaterialAssests : MonoBehaviour
{
    public string materialsLabel = "SandLevel"; // 设置标签来加载材质
    public static Material[] MaterialAssests; //材質

    async void Start()
    {
        bool isLoaded = await LoadMaterialsWithLabel();
    }
    public async Task<bool> LoadMaterialsWithLabel()
    {
        try
        {
            var handle = Addressables.LoadAssetsAsync<Material>(materialsLabel, null);
            await handle.Task; // 等待异步操作完成

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 获取所有加载的材质
                MaterialAssests = handle.Result.ToArray();
                return true;
            }
            else
            {
                Debug.LogError("Failed to load materials with label.");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception occurred while loading materials: {ex.Message}");
            return false;
        }
    }

 
   
}
