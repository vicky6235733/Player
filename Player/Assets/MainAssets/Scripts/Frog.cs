using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Frog : MonoBehaviour
{
    //
    FrogDataStruct frogStruct;
    PaperItemStruct paperStruct;
    ColorPapaerStruct colorStruct;
    GameObject CurrentTarget;

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
    public static bool isPaperSearched;

    bool isBigJumping; //和isMovingFrog同個東西，用來偵測大青蛙的，避免update一直跑

    //
    //CameraShake CameraShake; //鏡頭抖動腳本
    PaperFly call; //紙消失腳本

    void Start()
    {
        frogStruct = this.gameObject.GetComponent<FrogData>().frog;
        isMovingFrog = false;
        //


        //初始化目標點
        TargetPosition = EndPoint;
        //
        MoveSpeed = 4f;
        JumpHeight = 1.5f;
        isMovingFrog = false;
        isMovedToEnd = false;
        isPaperSearched = false;
        isBigJumping = false;
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

    public void MoveFrog()
    {
        isMovingFrog = true;
        isBigJumping = true;

        FrogTargetCounting();

        StartCoroutine(JumpToTarget());
    }

    void FrogTargetCounting()
    {
        if (paperStruct.PatternType != null)
        {
            colorStruct = FindObjectsOfType<ColorData>()
                .Where(ColorData =>
                    ColorData.colorPaper.PatternType != null
                    && ColorData.colorPaper.PatternType == paperStruct.PatternType
                )
                .Select(ColorData => ColorData.colorPaper)
                .FirstOrDefault();

            if (colorStruct.Activity == true)
            {
                if (transform.position == StartPoint)
                {
                    EndPoint = Point[2].position;
                    JumpHeight = 5f;
                }
            }
        }
    }

    public virtual IEnumerator JumpToTarget()
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
        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        if (frogStruct.State == 1)
        {
            if (
                other.gameObject != null
                && other.gameObject.tag == "Paper"
                && isPaperSearched == false
            )
            {
                CurrentTarget = other.gameObject;
                paperStruct = CurrentTarget.GetComponent<PaperData>().paper;
                string paperColor = CurrentTarget.GetComponent<PaperData>().paper.Color;

                if (frogStruct.Color == paperColor) //青蛙和貼紙顏色匹配
                {
                    Invoke("WaitedForCall", 1f);
                    isPaperSearched = true;
                }
            }
        }
    }

    void WaitedForCall()
    {
        call.callDisappear(CurrentTarget);
        CurrentTarget.GetComponent<PaperData>().paper.State = true;
    }
}
