using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFly : MonoBehaviour
{
    PaperData _pd; //貼紙組件
    PaperItemStruct ItemStruct; //貼紙資料
    float Range; //漂浮高度
    float FadeDuration; //時間

    Renderer ren;

    void Start()
    {
        Range = 0.5f;
        FadeDuration = 3f;
    }

    // Update is called once per frame
    public void callDisappear(GameObject item)
    {
        _pd = item.GetComponent<PaperData>();
        ItemStruct = _pd.paper;
        string _group = ItemStruct.GroupName;
        //
        List<GameObject> GroupPaper = GetObjectsWithGroupName(_group); //作用物件

        //遍歷相同組的貼紙
        foreach (GameObject i in GroupPaper)
        {
            ren = i.GetComponent<Renderer>();
            
            StartCoroutine(FadeOut(i, ren, ren.material.color));
        }
    }

    IEnumerator FadeOut(GameObject item, Renderer re, Color color) //對不同物件做處理
    {
        float elapsedTime = 0;
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        Color initialColor = color;
        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (re != null)
            {
                float lerp = Mathf.Clamp01(elapsedTime / FadeDuration);
                Color newColor = initialColor;
                newColor.a = Mathf.Lerp(color.a, 0, lerp); // 渐变到完全透明
              
                re.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetColor("_BaseColor", newColor);
                re.SetPropertyBlock(materialPropertyBlock);
            }
            item.transform.position = new Vector3(
                item.transform.position.x,
                item.transform.position.y + Range * Time.deltaTime,
                item.transform.position.z
            );

            yield return null;
        }

        if (re != null && elapsedTime >= FadeDuration)
        {
            item.SetActive(false);
            
            yield break;
        }
    }

    public static List<GameObject> GetObjectsWithGroupName(string groupName)
    {
        List<GameObject> objectsWithGroupName = new List<GameObject>();

        // 获取场景中所有的 PaperData 组件
        PaperData[] allPaperData = FindObjectsOfType<PaperData>();

        foreach (var paperData in allPaperData)
        {
            if (paperData.paper.GroupName == groupName)
            {
                //Debug.Log(groupName);
                objectsWithGroupName.Add(paperData.gameObject);
            }
        }

        return objectsWithGroupName;
    }
}
