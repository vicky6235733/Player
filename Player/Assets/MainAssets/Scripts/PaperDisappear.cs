using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperDisappear : MonoBehaviour
{

    public GameObject Paper;
    Transform PaperPosition;
    PaperItem TargetItem;
    //
    float Range;//漂浮高度
    float FadeDuration = 3.0f;
    float elapsedTime = 0f;//開始時間
    //
    Renderer _ren;
    Color StartColor;

    void Start()
    {
        Range = 0.75f;
         _ren = Paper.GetComponent<Renderer>();
        if (_ren != null)
        {
            StartColor = _ren.material.color;
        }
    }

  
    void Update()
    {
      elapsedTime += Time.deltaTime;
        if (_ren != null)
        {
            float lerp = Mathf.Clamp01( elapsedTime / FadeDuration);
            Color newColor = StartColor;
            newColor.a = Mathf.Lerp(StartColor.a, 0, lerp); // 渐变到完全透明
            _ren.material.color = newColor;
            transform.position = new Vector3(transform.position.x, transform.position.y + Range * Time.deltaTime, transform.position.z);
            
            if (lerp >= 1.0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
    
   
}
