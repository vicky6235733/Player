using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrogJumpControl : Frog{

public override IEnumerator JumpToTarget(){
    yield break;

}


}
public class BigSandFrog : MonoBehaviour
{

    FrogJumpControl frogJumpControl;
    int[] PaperLines = { 18, 17, 16, 15, 14, 19, 27, 23, 24, 20, 26, 28, 3, 2, 7, 1, 8, 9, 12, 13 }; //撕貼紙順序
    public Transform[] StickeTransforms = new Transform[20];
    public FrogDataStruct[,] FrogList; //二維陣列
    int road;
    public GameObject[] Frogs = new GameObject[9];
    int[] cnt = { 0,0,0,0,0,0,0,0,0};
    void Awake(){
        for(int i = 0; i < Frogs.Length; i++)
        {
            Frogs[i].GetComponent<FrogData>().frog.State = 2;
        }
    }
    void Start()
    {

        road = UnityEngine.Random.Range(0, 1);
        frogJumpControl = new FrogJumpControl();
        FrogList = new FrogDataStruct[PaperLines.Length, 3];

        for (int n = 0; n < FrogList.GetLength(0); n++)
        {
            for (int m = 0; m < FrogList.GetLength(1); m++)
            {
                FrogList[n, m] = new FrogDataStruct(false);
            }
        }
        //
        var matchingFrogs = FindObjectsOfType<FrogData>()
            .Where(frogData => frogData.frog.PaperTarget != null)
            .Select(frogData => frogData.frog); //找尋有被指派目標的青蛙

        foreach (var frog in matchingFrogs)
        {
            for (int i = 0; i < frog.PaperTarget.Length; i++) //將青蛙依照被指派的位置存到陣列
            {
                for (int p = 0; p < FrogList.GetLength(1); p++)
                {
                    if (
                        FrogList[Array.IndexOf(PaperLines, frog.PaperTarget[i]), p].isInLine
                        == false
                    )
                    {
                        FrogList[Array.IndexOf(PaperLines, frog.PaperTarget[i]), p] = frog;
                        break;
                    }
                }
            }
        }
        
        StartCoroutine(CheckFrogState());
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(StayInLines());
       
    }

    IEnumerator StayInLines() //每三秒撕一次貼紙
    {
        for (int i = 0; i < FrogList.GetLength(0); i++)
        {
            for (int j = 0; j < FrogList.GetLength(1); j++)
            {
                if (FrogList[i, j].isInLine == true)
                {
                    FrogList[i, j].State = 1;
                }

                yield return new WaitForSeconds(4f);
            }
        }
    }

    IEnumerator CheckFrogState() //定時檢查青蛙狀態(沒跳就讓他跳)
    {
        int i=0;
        while(true){
            Debug.Log(i);
            if (Frogs[i].GetComponent<FrogData>().frog.isJumping == false)
            {
                FrogMovitation(i);
            }
            if(i<8)
                {
                    i++;
                }else{
                    i=0;
                }
            yield return new WaitForSeconds(.4f);
        }
    }

    void FrogMovitation(int num) //決定路線
    {
       
        switch (Frogs[num].GetComponent<FrogData>().frog.State)
        {
            case 1:
                StickerJump(num);
                break;
            case 2:
                NPCJump(num);
                break;

            default:
                NPCJump(num);
                break;
        }
    }

    void NPCJump(int num) //青蛙預設兩條路
    {
        GameObject currentFrog = Frogs[num];

        if (road == 0)
        {
            /*currentFrog.GetComponent<Frog>().StartPoint = currentFrog
                .GetComponent<FrogData>()
                .frog.TargetLine[0 + cnt[num]]
                .position;
            currentFrog.GetComponent<Frog>().EndPoint = currentFrog
                .GetComponent<FrogData>()
                .frog.TargetLine[1 + cnt[num]]
                .position;*/
        }
        if (road == 1)
        {
            /*currentFrog.GetComponent<Frog>().StartPoint = currentFrog
                .GetComponent<FrogData>()
                .frog.TargetLine02[0 + cnt[num]]
                .position;
           / currentFrog.GetComponent<Frog>().EndPoint = currentFrog
                .GetComponent<FrogData>()
                .frog.TargetLine02[1 + cnt[num]]
                .position;*/
        }
        if (cnt[num] == 2)
        {
            road = UnityEngine.Random.Range(0, 1);
        }
        
        //currentFrog.GetComponent<FrogData>().frog.isJumping = true;

        if(cnt[num]>2){
            cnt[num] = 0;
        }
        else{
            cnt[num]++;
        }
    }

    void StickerJump(int num) //用貼紙編號決定青蛙
    {
        GameObject currentFrog = Frogs[num];
        
        //
        //currentFrog.GetComponent<FrogData>().frog.isJumping = true;
    }
}
