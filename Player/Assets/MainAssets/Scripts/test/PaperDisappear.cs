using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperDisappear : MonoBehaviour
{
    PaperManager _pm;

    //
    float Range; //漂浮高度
    float FadeDuration;


    void Start()
    {
        _pm = this.gameObject.GetComponent<PaperManager>();
        if (_pm == null)
        {
            Debug.Log("PaperManager not found");
        }
    }

    public void callDisappear(GameObject item)
    {
        //初始化觸碰到的物件，找到物件的PaperItem類
        PaperItem p = _pm.FindPaperItemByName(item.name); //改呼叫緩存已經比對完的方法
        //
        string _group = p.GroupName;
        List<PaperItem> AllPaper = _pm.GetAllPaperItems();
        List<GameObject> GroupPaper = new List<GameObject>(); //作用物件
        GroupPaper.Add(item);

        foreach (PaperItem i in AllPaper)
        {
            if (i.GroupName == _group)
            {
                GameObject paper = GameObject.Find(i.ObjectName); //不要用scriptableobject的資產，要引用場景中的物件*
                GroupPaper.Add(paper); //找到相同組的貼紙
            }
        }
         //初始設定(顏色、高度)
        if (p != null)
        {
            Range = p.Range;
            FadeDuration = p.FadeDuration;
        }
        else
        {
            Debug.Log("FindPaperItemByName broken or not found");
            return;
        }

        //遍歷相同組的貼紙
        foreach (GameObject i in GroupPaper)
        {
            Renderer renderer = i.GetComponent<Renderer>();
            StartCoroutine(FadeOut(i, renderer, renderer.material.color)); 
        }
    }

    IEnumerator FadeOut(GameObject item, Renderer re, Color color) //對不同物件做處理
    {
        float elapsedTime = 0;
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (re != null)
            {
                float lerp = Mathf.Clamp01(elapsedTime / FadeDuration);
                Color newColor = color;
                newColor.a = Mathf.Lerp(color.a, 0, lerp); // 渐变到完全透明
                re.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetColor("_Color", newColor);
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
}
