using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Frog : MonoBehaviour
{
    //
    FrogData frogStruct;

    //
    Animator anim;
    public Transform[] Point; //初始位置
    public Vector3 StartPoint; //起點
    public Vector3 EndPoint; //終點
    float MoveSpeed;
    float JumpHeight;

    //
    Vector3 TargetPosition; //當前目標
    bool isMovingFrog;
    bool isMovedToEnd;
    public static bool alreadyJumped;

    //
    //CameraShake CameraShake; //鏡頭抖動腳本
    PaperFly call; //紙消失腳本

    void Start()
    {
        frogStruct = this.gameObject.GetComponent<FrogData>();
        alreadyJumped = false;
        //
        if (Point != null)
        {
            StartPoint = Point[0].position;
            EndPoint = Point[1].position;
        }

        //初始化目標點
        TargetPosition = EndPoint;
        //
        MoveSpeed = 4f;
        JumpHeight = 1.5f;
        isMovingFrog = false;
        isMovedToEnd = false;

        //
        anim = GetComponent<Animator>();
        //CameraShake = Camera.main.GetComponent<CameraShake>();
        call = FindObjectsOfType<PaperFly>().FirstOrDefault();
    }

    // 青蛙朝向要調整預設值
    void Update()
    {
        if (!isMovingFrog)
        {
            MoveFrog();
            
        }
      
        
    }

    void MoveFrog()
    {
        isMovingFrog = true;
        FrogTargetCounting();
      
        switch (frogStruct.Number)
        {
            case 10:
                CheckColorPaper(10);
                StartCoroutine(JumpToTarget());
                break;

            case 11:
                if (alreadyJumped)
                {
                    MoveSpeed =10;
                    JumpHeight = 3f;
                    CheckColorPaper(11);
                    StartCoroutine(JumpToTarget());
                }
                else
                {
                    //Debug.Log("child is not frog 12,waiting...");
                    isMovingFrog = false;
                }
                break;

            case 12:
                if (!alreadyJumped)
                {
                    MoveSpeed =8;
                    StartCoroutine(JumpToTarget());
                }
                break;

            default:
                Debug.Log("frog number of struct is error");
                isMovingFrog = false;
                break;
        }
    }

    void FrogTargetCounting() //小青蛙偵測是否已經填色
    {
        if (frogStruct.paperStruct.PatternType != null) //如果有踩過目標，就分配目標顏色然後交給小青蛙偵測有沒有填
        {
            frogStruct.colorStruct = FindObjectsOfType<ColorData>()
                .Where(ColorData =>
                    ColorData.colorPaper.PatternType == frogStruct.paperStruct.PatternType
                )
                .Select(ColorData => ColorData.colorPaper)
                .FirstOrDefault();
        }
    }

    void CheckColorPaper(int num) //每次移動前偵測是否改目標，有填就更改
    {
        switch (num)
        {
            case 10:
                if (frogStruct.colorStruct.Color != null)
                {
                    if (frogStruct.colorStruct.Activity == true)
                    {
                        if (transform.position == StartPoint)
                        {
                            EndPoint = Point[2].position;
                            JumpHeight = 5f;
                        }
                    }
                }
                break;
            case 11:
                if (frogStruct.colorStruct.Color != null)
                {
                    if (frogStruct.colorStruct.Activity == true)
                    {
                        if (transform.position == StartPoint)
                        {
                            EndPoint = Point[2].position;
                            JumpHeight = 5.5f;
                        }
                    }
                }
                break;

            default:
                Debug.Log("frog number in function error");
                break;
        }
    }

    IEnumerator JumpToTarget()
    {
        Vector3 StartPosition = transform.position; //在一次跳躍中的起點
        TargetPosition = isMovedToEnd ? StartPoint : EndPoint; //在一次跳躍中的終點

        Vector3 directionToTarget = (TargetPosition - StartPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1f); // 立即朝向目標
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); //校正XZ旋轉

        float MoveDistance = Vector3.Distance(transform.position, TargetPosition); //距離長度
        float JumpDuration = MoveDistance / MoveSpeed; //移動時間
        float now = 0;

        anim.SetTrigger("BFrogJump");
        yield return new WaitForSeconds(.6f);

        while (now < JumpDuration)
        {
            now += Time.deltaTime;
            float t = now / JumpDuration; //當前時間段數值

            // 改變曲線使起跳快而下降慢
            float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f); // 起跳快，下降慢
            float CurrentHeight = Mathf.Sin(t * Mathf.PI) * JumpHeight; // 拋物線高度

            transform.position =
                Vector3.Lerp(StartPosition, TargetPosition, curvedT)
                + new Vector3(0, CurrentHeight, 0);

            yield return null;
        }
        transform.position = TargetPosition;

        /*if (CameraShake != null)
        {
            StartCoroutine(CameraShake.Shake(0.65f, 0.35f));
        }*/

        yield return new WaitForSeconds(2f);

        //交換起點終點位置 狀態
        isMovedToEnd = !isMovedToEnd;
        isMovingFrog = false;

        if (frogStruct.Number == 12)
        {
            this.gameObject.transform.SetParent(Point[1].parent);
            isMovingFrog = true;
            alreadyJumped = true;
        }
    }
}
