using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PaperData", menuName = "SandItem/PaperData", order = 1)]
public class PaperItem : ScriptableObject
{
    public string PatternType; //形狀
    public string Color; //顏色
    public float Size=5f; //大小
    public GameObject Paper; //紙
    public Transform CurrentTransfrom; //位置
    public bool State; //狀態
    public int ID; //編號
    

    //
    float Range = 0.5f; //漂浮高度
    float FadeDuration = 3.0f;
    float elapsedTime = 0f; //開始時間

    //
    public Renderer _ren;
    public Color StartColor;

    public PaperItem()
    { //建構子
       
    }

    
}
