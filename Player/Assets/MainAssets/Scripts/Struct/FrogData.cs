using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrogDataStruct
{
    public int Number; //青蛙編號
    public int State; //切換NPC或是撕貼紙狀態
    public string Color; //青蛙顏色
    public Vector3 InitialPoint; //初始位置
    public Vector3 CurrentPoint; //目前位置
    public Vector3 TargetPoint; //目標位置

    public FrogDataStruct(int num, Vector3 ip, string color)
    {
        this.Number = num;
        this.State = 1; //1:NPC or 2:Sticker
        this.Color = color;
        this.InitialPoint = ip;
        this.CurrentPoint = ip;
        this.TargetPoint = Vector3.zero;
    }
}

public class FrogData : MonoBehaviour
{
    public int Number;
    Vector3 _ip;
    public string Color;

    void Start()
    {
        _ip = transform.position;

        if (Number != 0 && Color != null)
        {
            FrogDataStruct frog = new FrogDataStruct(Number, _ip, Color);
        }
    }
}
