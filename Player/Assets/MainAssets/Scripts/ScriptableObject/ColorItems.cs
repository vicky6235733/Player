using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "SandItem/ColorData", order = 1)]
public class ColorItem : ScriptableObject
{
    public string PatternType; //形狀
    public string Color; //顏色
    public string GroupName;//一起撕的貼紙
    public float Size=5f; //大小
    public GameObject ColorPaper; //紙
    public Transform CurrentTransfrom; //位置
    public bool Activity; //狀態
    public int ID; //編號
    

    
}
