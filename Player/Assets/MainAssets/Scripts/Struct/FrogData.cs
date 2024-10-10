using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct FrogDataStruct
{
    public int Number; //青蛙編號
    public int State; //切換NPC或是撕貼紙狀態
    public string Color; //青蛙顏色
    public Transform[] TargetLine; //目標位置
    public Transform[] TargetLine02; //目標位置
    public int[] PaperTarget; //負責貼紙編號
    public bool isJumping; //是否跳回起點
    public bool isInLine;
    public bool SmallFrog;
    public int CurrentTargetPaper; //目前目標貼紙

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
        this.SmallFrog = false;
        this.CurrentTargetPaper = -1;
    }

    public FrogDataStruct(
        int num,
        Transform[] point,
        Transform[] point02,
        string color,
        int[] tp,
        bool small
    )
    {
        this.Number = num;
        this.State = 2; //1:Sticker or 2:NPC
        this.Color = color;
        this.TargetLine = point;
        this.TargetLine02 = point02;
        this.PaperTarget = tp;
        this.isJumping = false;
        this.isInLine = true;
        this.isJumping = false;
        this.SmallFrog = small;
        CurrentTargetPaper = -1;
    }
}

public class FrogData : MonoBehaviour
{
    //struct data
    public int Number;
    public string Color;
    public FrogDataStruct frog;
    public int[] targetPaper;
    public bool isSmallFrog;
    public Transform[] _point = new Transform[4]; //第一條路
    public Transform[] _point02 = new Transform[4]; //第二條路

    //
    GameObject CurrentTarget;
    public PaperItemStruct paperStruct;
    public ColorPapaerStruct colorStruct;
    PaperFly call; //紙消失腳本
    public bool isStateLocked = false;
    //
    Rigidbody rb;

    void Awake()
    {
        if (Number != 0 && Color != null)
        {
            frog = new FrogDataStruct(Number, _point, _point02, Color, targetPaper, isSmallFrog);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        call = FindObjectsOfType<PaperFly>().FirstOrDefault();
    }

    void FixedUpdate()
    {
        // 每幀施加重力
        rb.velocity += Vector3.down * 9.8f * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider other) //偵測到匹配對象並讓貼紙call消失
    {
        if (isSmallFrog) //小青蛙
        {
            if (other.gameObject.tag == "Paper")
            {
                CurrentTarget = other.gameObject;
                paperStruct = CurrentTarget.GetComponent<PaperData>().paper;
                string paperColor = CurrentTarget.GetComponent<PaperData>().paper.Color;

                if (frog.Color == paperColor) //青蛙和貼紙顏色匹配
                {
                    Invoke("WaitedForCall", 1f);
                }
            }
        }
        else if (isStateLocked)
        { //大青蛙
            if (other.gameObject.tag == "Paper")
            {
                PaperData? isPaper = other.gameObject.GetComponent<PaperData>();
                int PaperID = 0;
                if (isPaper != null)
                {
                    PaperID = isPaper.paper.ID - 30;
                }

                if (frog.CurrentTargetPaper == PaperID)
                {
                    CurrentTarget = other.gameObject;

                    Invoke("WaitedForCall", 1f);
                    isStateLocked = false;
                }
            }
        }
        
        //Invoke("StopDrift", 0.5f);
    }

    void WaitedForCall() //呼叫貼紙消失
    {
        call.callDisappear(CurrentTarget);
        CurrentTarget.GetComponent<PaperData>().paper.State = true; //表示貼紙飄起
    }

    void StopDrift()//取消kinematic用
    {
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
