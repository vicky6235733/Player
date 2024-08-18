using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//類別命名和editor不能共用，所以需要分開定義兩次
[System.Serializable]
public class PositionData
{
    public string ObjectName; // 改為 string，因為 GameObject 無法直接序列化為 JSON
    public Vector3 Position;
    public Quaternion Rotation;
}

[System.Serializable]
public class PositionDataList
{
    public List<PositionData> positions = new List<PositionData>();
}

public class PaperManager : MonoBehaviour
{
    public PaperItem[] PaperItems = new PaperItem[28];

    string jsonFilePath = @"Assets\MainAssets\Prefabs\positionData.json"; // JSON 文件路徑

    private Dictionary<string, Vector3> positionDataMap = new Dictionary<string, Vector3>();
    private Dictionary<string, Quaternion> rotationDataMap = new Dictionary<string, Quaternion>();


    //記得用editor儲存所有位置之後才能引用位置資訊

    //未實裝貼紙撕掉功能
    //frog目前沒呼叫方法只有碰撞
    //原始物件未刪除 等成功之後再刪除改用實例化

    void Start()
    {
        LoadPositionData();
        foreach (PaperItem p in PaperItems)
        {
            // 初始化貼紙參數

            if (p.Paper != null)
            {
                p.CurrentTransfrom = p.Paper.transform;
                p._ren = p.Paper.GetComponent<Renderer>();
                if (positionDataMap.TryGetValue(p.Paper.name, out Vector3 po))
                {
                    p.CurrentTransfrom.position = po;
                }
                else
                {
                    Debug.LogWarning($"Position data for {p.name} not found.");
                }
                if (rotationDataMap.TryGetValue(p.Paper.name, out Quaternion ro))
                {
                    p.CurrentTransfrom.rotation = ro;
                }
                else
                {
                    Debug.LogWarning($"Rotation data for {p.name} not found.");
                }
                if (p._ren != null)
                { //rednerer and color
                    p.StartColor = p._ren.sharedMaterial.color;
                }
                else
                {
                    Debug.Log("Failed to load paper data.");
                }
            }
            else
            {
                Debug.Log("PaperObject is null");
            }
            RenderPaper(p);
        }
    }

    void RenderPaper(PaperItem p)
    {
        if (p.Paper != null)
        {
            GameObject newParent = GameObject.Find(p.PatternType);
            // 實例化貼紙對應的 3D 模型
            GameObject PaperObject = Instantiate(p.Paper);
            PaperObject.name = p.PatternType + "_" + p.ID;
            PaperObject.tag = "Paper";
            if (p.CurrentTransfrom != null && p.CurrentTransfrom.gameObject.scene.IsValid())
            {
                PaperObject.transform.SetParent(newParent.transform); // 手動設置為父物件，因為靜態資產無法直接實例化成為場景最高層級物件
                PaperObject.transform.localPosition = newParent.transform.InverseTransformPoint(
                    p.CurrentTransfrom.position
                ); //將世界座標轉換為本地座標
                PaperObject.transform.localRotation = newParent.transform.rotation;
            }

            // 設置模型的材質、貼圖等參數
            MeshRenderer renderer = PaperObject.GetComponent<MeshRenderer>();

            // 設置模型的大小位置
            PaperObject.transform.localScale = new Vector3(p.Size, p.Size, p.Size);
            PaperObject.transform.position = p.CurrentTransfrom.position;
            PaperObject.transform.rotation = p.CurrentTransfrom.rotation;

            //增加碰撞
            if(PaperObject.GetComponent<BoxCollider>() == null)
            {
                BoxCollider AddedCollider = PaperObject.AddComponent<BoxCollider>();
                Vector3 originalSize = AddedCollider.size;
                float newHeight = renderer.bounds.size.y + 0.01f;

                AddedCollider.size = new Vector3(originalSize.x, newHeight, originalSize.z);

            }
        }
        else
        {
            Debug.Log("Failed to catch PaperObject.");
        }
    }

    void LoadPositionData()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            PositionDataList positionDataList = JsonUtility.FromJson<PositionDataList>(json);

            if (positionDataList != null)
            {
                positionDataMap.Clear();
                rotationDataMap.Clear();

                foreach (PositionData data in positionDataList.positions)
                {
                    positionDataMap[data.ObjectName] = data.Position;
                    rotationDataMap[data.ObjectName] = data.Rotation;
                }
            }
            else
            {
                Debug.LogError("Failed to parse JSON data.");
            }
        }
        else
        {
            Debug.LogError("JSON file not found.");
        }
    }
}
