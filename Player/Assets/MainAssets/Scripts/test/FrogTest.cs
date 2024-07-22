using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogTest : MonoBehaviour
{
    public Transform StartObject;
    public Transform EndOnject;
    Vector3 StartPoint;//起點
    Vector3 EndPoint;//終點
    float MoveSpeed;
    float JumpHeight;
    //
    Vector3 TargetPosition;//當前目標
    bool isMovingFrog;
    bool isMovedToEnd;
    //
    CameraShake CameraShake;//鏡頭抖動腳本

    

    // Start is called before the first frame update
    void Start()
    {
        //
        StartPoint=StartObject.position;
        EndPoint=EndOnject.position;
        //初始化目標點
        TargetPosition = EndPoint;
        //
        MoveSpeed = 5f;
        JumpHeight = 5f;
        isMovingFrog=false;
        isMovedToEnd=false;
        //
        CameraShake = Camera.main.GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMovingFrog){
             MoveFrog();
        }
    }
   
    
    void MoveFrog(){
        


        isMovingFrog =true;
        StartCoroutine(JumpToTarget());

    }
     IEnumerator JumpToTarget(){//在update每偵更新插值會有時間衝突

        Vector3 StartPosition = transform.position;//在一次跳躍中的起點
        TargetPosition = isMovedToEnd ? StartPoint : EndPoint;//在一次跳躍中的終點

        //計算拋物線
        float MoveDistance = Vector3.Distance(transform.position, TargetPosition);//距離長度
        float JumpDuration = MoveDistance / MoveSpeed ;//移動時間
        float now=0;

        while(now < JumpDuration){

            now += Time.deltaTime;
            float t = now / JumpDuration;//當前時間段數值
            float CurrentHeight= Mathf.Sin(t * Mathf.PI) * JumpHeight;//用數學取出當前時段必須在的弧線高度
            transform.position = Vector3.Lerp(StartPosition, TargetPosition, t) + new Vector3(0, CurrentHeight, 0);
            
            yield return null;  
        }
        transform.position = TargetPosition;

        if (CameraShake != null)
            {
                StartCoroutine(CameraShake.Shake(0.5f, 0.25f));
            }
            
         yield return new WaitForSeconds(2f);

         //交換起點終點位置 狀態
        isMovedToEnd = !isMovedToEnd;
        isMovingFrog = false;
        


    
        
    }
}
