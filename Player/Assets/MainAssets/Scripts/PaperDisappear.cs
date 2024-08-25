using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperDisappear : MonoBehaviour
{
    PaperManager _pm;

    //
    float Range; //漂浮高度
    float FadeDuration;
    float elapsedTime; //開始時間

    //
    Renderer _ren;
    Color StartColor;

    void Start()
    {
        _pm = this.gameObject.GetComponent<PaperManager>();
        if(_pm == null){
            Debug.Log("PaperManager not found");
        }
    }
    public void callDisappear(GameObject item)
    {
        //初始化觸碰到的物件，找到物件的PaperItem類
        PaperItem p = _pm.FindPaperItemByName(item.name); //改呼叫緩存已經比對完的方法
        if (p != null)
        {
            Range = p.Range;
            FadeDuration = p.FadeDuration;
            elapsedTime = p.elapsedTime;
            _ren = p._ren;
            StartColor = p.StartColor;
        }else{
            Debug.Log("FindPaperItemByName broken or not found");
            return;
        }

        //
        StartCoroutine(FadeOut(item));
       
    }
    IEnumerator FadeOut(GameObject item){
         while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            if (_ren != null)
            {
                float lerp = Mathf.Clamp01(elapsedTime / FadeDuration);
                Color newColor = StartColor;
                newColor.a = Mathf.Lerp(StartColor.a, 0, lerp); // 渐变到完全透明
                _ren.sharedMaterial.color = newColor;
                item.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + Range * Time.deltaTime,
                    transform.position.z
                );

                if (lerp >= 1.0f)
                {
                    item.SetActive(false);
                    Frog.isPaperSearched = false;
                    yield break;
                }
            }else{
                yield break;;
            }
        }
    }
}
