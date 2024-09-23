using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigSandFrog : MonoBehaviour
{
    int[] PaperLines = { 18, 17, 16, 15, 14, 19, 27, 23, 24, 20, 26, 28, 3, 2, 7, 1, 8, 9, 12, 13 }; //撕貼紙順序
    public Transform[] StickeTransforms = new Transform[20];
    public FrogDataStruct[,] FrogList; //二維陣列
    int road;
    public GameObject[] Frogs = new GameObject[9];
    int[] cnt = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int i = -1; //控制協程i
    PaperFly call; //紙消失腳本
    Animator anim;

    void Awake()
    {
        for (int i = 0; i < Frogs.Length; i++)
        {
            Frogs[i].GetComponent<FrogData>().frog.State = 2; //初始狀態設定不撕貼紙
        }
    }

    void Start()
    {
        call = FindObjectsOfType<PaperFly>().FirstOrDefault();
        //初始化陣列
        FrogList = new FrogDataStruct[PaperLines.Length, 3];

        for (int n = 0; n < FrogList.GetLength(0); n++)
        {
            for (int m = 0; m < FrogList.GetLength(1); m++)
            {
                FrogList[n, m] = new FrogDataStruct(false);
            }
        }
        //找尋有被指派目標的青蛙
        var matchingFrogs = FindObjectsOfType<FrogData>()
            .Where(frogData => frogData.frog.PaperTarget != null)
            .Select(frogData => frogData.frog);

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
    }

    void Update()
    {
        if (i == -1)
        {
            i++;
            StartCoroutine(CheckFrogState());
        }
        //StartCoroutine(StayInLines());
    }

    IEnumerator StayInLines() //每三秒撕一次貼紙
    {
        for (int i = 0; i < FrogList.GetLength(0); i++)
        {
            for (int j = 0; j < FrogList.GetLength(1); j++)
            {
                if (FrogList[i, j].isInLine == true) //如果struct不為空值
                {
                    FrogList[i, j].State = 1;
                }

                yield return new WaitForSeconds(3f);
            }
        }
    }

    IEnumerator CheckFrogState() //定時開啟青蛙跳直到所有青蛙都在跳
    {
        while (true)
        {
            if (Frogs[i].GetComponent<FrogData>().frog.isJumping == false) //回到原點才判斷狀態
            {
                FrogMovitation(i);
            }
            else
            {
                yield return null; // 讓協程暫停一個幀，避免無限迴圈
                continue;
            }

            if (i < 8)
            {
                i++;
            }
            else
            {
                i = -1;
                yield break;
            }

            yield return new WaitForSeconds(.4f);
        }
    }

    void FrogMovitation(int num) //決定狀態
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
        if (cnt[num] == 0)
        {
            road = UnityEngine.Random.Range(0, 1);
        }
        GameObject currentFrog = Frogs[num];
        FrogDataStruct currentFrogData = currentFrog.GetComponent<FrogData>().frog;

        if (road == 0)
        {
            StartCoroutine(JumpToTarget(currentFrogData.TargetLine, currentFrog, 0));
        }
        if (road == 1)
        {
            StartCoroutine(JumpToTarget(currentFrogData.TargetLine02, currentFrog, 0));
        }

        if (cnt[num] >= 4)
        {
            cnt[num] = 0;
        }
        else
        {
            cnt[num]++;
        }
    }

    IEnumerator StickerJump(int num) //用貼紙編號決定青蛙
    {
        GameObject currentFrog = Frogs[num];
        yield break;
    }

    IEnumerator JumpToTarget(Transform[] Point, GameObject frog, int current) //路線，青蛙，青蛙目前站點
    {
        anim = frog.GetComponent<Animator>();
        anim.SetBool("FrogJump", true);

        //狀態設定
        float MoveSpeed = 4f; //移動速度
        float JumpHeight = 1.5f;
        int totalPoints = Point.Length; // 獲取點的總數
        Vector3 StartPosition = Point[current].position; //在一次跳躍中的起點
        Vector3 TargetPosition;
        if (current < 3)
        {
            TargetPosition = Point[current + 1].position; //在一次跳躍中的終點
        }
        else
        {
            TargetPosition = Point[0].position; //在一次跳躍中的終點
        }
        frog.GetComponent<FrogData>().frog.isJumping = true;

        //旋轉處理
        Vector3 directionToTarget = (TargetPosition - StartPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        float rotationSpeed = 200f; // 調整速度

        float MoveDistance = Vector3.Distance(StartPosition, TargetPosition); //距離長度
        float JumpDuration = MoveDistance / MoveSpeed; //移動時間
        float now = 0;

        //跳躍處理
        while (now < JumpDuration)
        {
            now += Time.deltaTime;
            float t = now / JumpDuration; //當前時間段數值

            frog.transform.rotation = Quaternion.RotateTowards(
                frog.transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
            frog.transform.eulerAngles = new Vector3(0, frog.transform.eulerAngles.y, 0); //校正XZ旋轉

            // 改變曲線使起跳快而下降慢
            float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f); // 起跳快，下降慢
            float CurrentHeight = Mathf.Sin(t * Mathf.PI) * JumpHeight; // 拋物線高度

            frog.transform.position =
                Vector3.Lerp(StartPosition, TargetPosition, curvedT)
                + new Vector3(0, CurrentHeight, 0);

            yield return null;
        }
        frog.transform.position = TargetPosition;
        frog.GetComponent<FrogData>().frog.current = TargetPosition;

        yield return new WaitForSeconds(2f);
        anim.SetBool("FrogJump", false);

        //更換目標

        current = (current + 1) % totalPoints; // 循環到下一點
        /*if (current == 0)
        { //跳過一輪就停止然後跳出
            frog.GetComponent<FrogData>().frog.isJumping = false;
            yield break;
        }*/
        StartCoroutine(JumpToTarget(Point, frog, current)); //重複呼叫直到跳完一輪
    }

    
}
