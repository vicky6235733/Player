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

    //
    //CameraShake CameraShake; //鏡頭抖動腳本
    PaperFly call; //紙消失腳本

    void Start()
    {
        frogStruct = this.gameObject.GetComponent<FrogData>();
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
            anim.SetBool("FrogJump", true);
        }
        else
        {
            anim.SetBool("FrogJump", false);
        }
    }

    void MoveFrog()
    {
        isMovingFrog = true;
        FrogTargetCounting();
        CheckColorPaper();
        StartCoroutine(JumpToTarget());
    }

    void FrogTargetCounting() //小青蛙偵測是否填色
    {
        if (frogStruct.paperStruct.PatternType != null) //如果有踩過目標，就分配目標顏色然後交給小青蛙偵測
        {
            frogStruct.colorStruct = FindObjectsOfType<ColorData>()
                .Where(ColorData =>
                    ColorData.colorPaper.PatternType == frogStruct.paperStruct.PatternType
                )
                .Select(ColorData => ColorData.colorPaper)
                .FirstOrDefault();
        }
    }

    void CheckColorPaper() //每次移動前偵測是否改目標
    {
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
    }
}
