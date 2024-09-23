using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrogDataStruct
{
    public int Number; //青蛙編號
    public int State; //切換NPC或是撕貼紙狀態
    public string Color; //青蛙顏色
    public Vector3 current; //目前位置
    public Transform[] TargetLine; //目標位置
    public Transform[] TargetLine02; //目標位置
    public int[] PaperTarget; //負責貼紙編號
    public bool isJumping; //是否跳回起點
    public bool isInLine;

    public FrogDataStruct(bool b)
    {
        this.Number = 0;
        this.State = 0;
        this.Color = null;

        this.TargetLine = null;
        this.TargetLine02 = null;
        this.PaperTarget = null;
        this.isJumping = false;
        this.isInLine = false;
        this.isJumping = false;
        this.current = new Vector3(0, 0, 0);
    }

    public FrogDataStruct(
        int num,
        Transform[] point,
        Transform[] point02,
        string color,
        int[] tp,
        Vector3 cur
    )
    {
        this.Number = num;
        this.State = 1; //1:Sticker or 2:NPC
        this.Color = color;
        this.TargetLine = point;
        this.TargetLine02 = point02;
        this.PaperTarget = tp;
        this.isJumping = false;
        this.isInLine = true;
        this.isJumping = false;
        this.current = cur;
    }
}

public class FrogData : MonoBehaviour
{
    public int Number;
    public string Color;
    public FrogDataStruct frog;
    public int[] targetPaper;
    public Vector3 _current;
    public Transform[] _point = new Transform[4]; //第一條路
    public Transform[] _point02 = new Transform[4]; //第二條路
    

    void Awake()
    {
        _current = this.transform.position;

        if (Number != 0 && Color != null)
        {
            frog = new FrogDataStruct(Number, _point, _point02, Color, targetPaper, _current);
        }
    }

    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    { //處理大沙畫青蛙
        
    }

    void WaitedForCall()
    {//把frog的搬過來
        
    }
}
