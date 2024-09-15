using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FrogDataStruct
{
    public int Number; //青蛙編號
    public int State; //切換NPC或是撕貼紙狀態
    public string Color; //青蛙顏色
   
    public Transform[] TargetLine; //目標位置
    public Transform[] TargetLine02; //目標位置
    public int[] PaperTarget; //負責貼紙編號
    public bool isBigFrog; 
    public bool isJumping; //是否正在跳躍(大青蛙控制用)
    public bool isInLine;
    
    public FrogDataStruct(bool b){
        this.Number = 0;
        this.State = 0;
        this.Color = null;

        this.TargetLine = null;
        this.TargetLine02 = null;
        this.PaperTarget = null;
        this.isJumping = false;
        this.isInLine = false;
        this.isBigFrog = false;
        this.isJumping = false;
    }
    public FrogDataStruct(int num, Transform[] point,Transform[] point02, string color, int[] tp,bool isb)
    {
        this.Number = num;
        this.State = 1; //1:Sticker or 2:NPC
        this.Color = color;
        this.TargetLine = point;
        this.TargetLine02 = point02;
        this.PaperTarget = tp;
        this.isJumping = false;
        this.isInLine = true;
        this.isBigFrog = isb;
         this.isJumping = false;
    }
}

public class FrogData : MonoBehaviour
{
    public int Number;

    public string Color;
    public FrogDataStruct frog;
    public int[] targetPaper;
    public bool isBig;

    public Transform[] _point=new Transform[4];//第一條路
    public Transform[] _point02=new Transform[4];//第二條路

    void Awake()
    {
       

        if (Number != 0 && Color != null)
        {
            frog = new FrogDataStruct(Number,_point,_point02, Color, targetPaper,isBig);
        }
    }
}
