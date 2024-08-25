using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{
    public PaperItem TargetItem;
    public Transform StartObject;
    public Transform EndOnject;
    Animator anim;
    Vector3 StartPoint;//起點
    Vector3 EndPoint;//終點
    float MoveSpeed;
    float JumpHeight;
    //
    Vector3 TargetPosition;//當前目標
    bool isMovingFrog;
    bool isMovedToEnd;
    public static bool isPaperSearched;

    //
    CameraShake CameraShake;//鏡頭抖動腳本
    PaperDisappear call;//紙消失腳本
    

    

    // Start is called before the first frame update
    void Start()
    {
        //
        StartPoint=StartObject.position;
        EndPoint=EndOnject.position;
        //初始化目標點
        TargetPosition = EndPoint;
        //
        MoveSpeed = 4f;
        JumpHeight = 1.5f;
        isMovingFrog=false;
        isMovedToEnd=false;
        isPaperSearched=false;
        //
        anim = GetComponent<Animator>();
        CameraShake = Camera.main.GetComponent<CameraShake>();
        call=GameObject.Find("PaperManager").GetComponent<PaperDisappear>();
        
    }

    // 青蛙朝向要調整預設值
    void Update()
    {
        if(!isMovingFrog){
             MoveFrog();
             anim.SetBool("FrogJump",true);
        }else{
            anim.SetBool("FrogJump",false);
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
                StartCoroutine(CameraShake.Shake(0.65f, 0.35f));
            }
            
         yield return new WaitForSeconds(2f);

         //交換起點終點位置 狀態
        isMovedToEnd = !isMovedToEnd;
        isMovingFrog = false;
        transform.Rotate(Vector3.up, 180f);
        
    }
    void OnCollisionEnter(Collision other) {
        
        if(other.gameObject.tag == "Paper" && isPaperSearched==false){
            
                call.callDisappear(other.gameObject);
                print("hit");
                isPaperSearched=true;
        }
    }
  
}
