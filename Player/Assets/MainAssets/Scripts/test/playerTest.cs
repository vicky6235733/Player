using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTest : MonoBehaviour
{   
    //
    Rigidbody rb;
    Vector3 MoveDirection;//方向數值
    Vector3 MoveAmount;//位移距離
    //
    LayerMask GroundLayer;//地板圖層
    float GroundRayLength;//地板射線長度
    LayerMask CollisionLayer;//牆壁圖層
    float CollisionRayLength;//牆壁射線長度
    float decelerationFactor; //减速因子
    //
    float MoveSpeed;//走路速度
    float Speed; //衝刺速度
    float RotateSpeed;
    float JumpForce;
    //撞牆反饋
    float knockBackAngle;//彈回角度
    float KnockBackForce;//彈回力度
    float KnockDackDuration;//彈回時間長短
    float knockBackKeeper;//彈回後的目前時間
    //
    bool isSlowDown;//是否減速
    bool isKnockBack;
    bool isSprinting;//是否正在衝刺
    bool isOnFloor;
    static bool ColliderEntered;
    //
    Animator anim;
    public static bool SpintAnimationRestart;//衝刺動畫是否能重新觸發(在update)
   

    void Start()
    {

        anim=GetComponent<Animator>();
        AnimaController(1f);
        //初始化
        rb = GetComponent<Rigidbody>();
        GroundLayer = LayerMask.GetMask("Ground");
        GroundRayLength = 1f;
        CollisionLayer = LayerMask.GetMask("Collision");
        CollisionRayLength = 2.5f;
        decelerationFactor = 0.75f;
        //
        MoveSpeed = 5f;
        Speed = MoveSpeed * 2;
        JumpForce = 500f;
        RotateSpeed = 100f;
        //
        MoveDirection = Vector3.zero;
        MoveAmount = Vector3.zero;
        //
        knockBackAngle = 180f;
        KnockBackForce = 100f;
        KnockDackDuration = 0.5f;
        knockBackKeeper = 0;
        //
        isKnockBack = false;
        isOnFloor = true;
        isSlowDown =false;
        ColliderEntered = false;
        isSprinting = false;//觸發update衝刺
        //
        SpintAnimationRestart=true;
 
 
       
        
    }
    //目前剩餘射線優化(在一個物件設置多個碰撞，並分別引用Layer，來同時偵測地板和牆壁)
    //衝刺動畫優化
    //跳躍動畫和idle衝突
    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);//用來讀動畫狀態的物件(第0個Layer)
        
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        MoveDirection = new Vector3(hor, 0, ver);
        if(MoveDirection == Vector3.zero){
            AnimaController(1f);
        }

        //rotate
        Vector3 RotateAmount = hor * Vector3.up * RotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(RotateAmount);
        rb.MoveRotation(rb.rotation * deltaRotation);
        

        //jump
        Vector3 position = transform.position + Vector3.up * 0.5f;
        Debug.DrawRay(position, Vector3.down * GroundRayLength, Color.red);
        isOnFloor = Physics.Raycast(position, Vector3.down, GroundRayLength, GroundLayer);//射線偵測下方有沒有地板
       
        if(isOnFloor && Input.GetKeyDown(KeyCode.Space))
        {
            AnimaController(2f);
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            
        }
       
       //sprint
            if (Input.GetKeyDown(KeyCode.R))
            {  
                isSprinting = true;//如果按R就判斷衝刺
                if(SpintAnimationRestart){
                    anim.SetBool("SprintStart",true);
                    SpintAnimationRestart=false;
                }
            }
           
        //walk  
            if (MoveDirection != Vector3.zero && !isSprinting)
            { 
                Vector3 MoveLength = new Vector3(0, 0, ver);
                rb.MovePosition(rb.position + (transform.forward * ver * MoveSpeed * Time.deltaTime));
                AnimaController(3f);
            }
                
            

        //slowDown(減速)
        isSlowDown = Physics.Raycast(transform.position, transform.forward, CollisionRayLength, CollisionLayer);//射線偵測前方有沒有物體

        //knockback(如果正在彈回，就倒數時間然後停止)
        if (isKnockBack)
        {
            knockBackKeeper -= Time.deltaTime;
            if (knockBackKeeper <= 0)
            {
                isKnockBack = false;
                ColliderEntered = false;
                rb.velocity = Vector3.zero;
            }
        }
    }
 
    void FixedUpdate()//衝刺用update因為更新太快、衝太快，會有發生更新FPS衝突
    {
        
        float ver = Input.GetAxis("Vertical");
        if(isSprinting)
        {
          
            if(MoveDirection != Vector3.zero){ //sprint_run(衝刺跑，有加入物理量)

                if(!isSlowDown){//前方有物體就不給他硬衝
                    MoveAmount = transform.forward * ver * Speed * Time.fixedDeltaTime *1.5f;
                    }
                }else{//sprint(單純衝刺)
                    MoveDirection = transform.forward;
                    MoveAmount = MoveDirection * Speed * Time.fixedDeltaTime;
                    
                }
                
        }
        
        //walk and sprint
        if (!ColliderEntered)
        {
            StartCoroutine(SprintProcess(MoveAmount));
        }
        

        //slowDown
        SlowDown();
    }
    void SlowDown(){//如果前方有物體在靠近，就讓玩家減速(避免玩家硬要撞穿模)
        rb.velocity = new Vector3(
                Mathf.Lerp(rb.velocity.x, 0, decelerationFactor * Time.fixedDeltaTime),
                rb.velocity.y,
                Mathf.Lerp(rb.velocity.z, 0, decelerationFactor * Time.fixedDeltaTime)
            );
    }
    void StopMove()//強制停下衝刺的物理慣性
    {

        MoveAmount = Vector3.zero;
        isSprinting = false;
       
    }
    IEnumerator SprintProcess(Vector3 Amount){
        yield return new WaitForSeconds(0.3f);
        while(Amount.magnitude > 0.1f){
            if(!anim.GetBool("SprintStart")){
                rb.MovePosition(rb.position + Amount);
                Invoke("StopMove", 0.8f);
                yield break;
            }else{
                yield return null;
            }
        }
        

    }
    
    //
    void AnimaController(float item){
        anim.SetFloat("Action", item);
    }
    //
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag != "floor")//如果撞到物體就停下，然後彈回
        {
            MoveAmount = Vector3.zero;//停止移動
            if (rb.velocity.magnitude > 0.1f)
            {
                rb.AddForce(-rb.velocity.normalized);//給予反向力讓玩家停止
            }else{  // 避免來回震動，將速度限制在接近零時直接設為零
                    rb.velocity = Vector3.zero;
                }
            ColliderEntered = true;
            KnockBack(other);//用other偵測前方物體位置，方便計算反彈角度
        }
    }
 

    void KnockBack(Collision target)//彈回
    {
        Vector3 KnockBackDiraction = (transform.position - target.transform.position).normalized;

        float radians = knockBackAngle * Mathf.Deg2Rad;
        KnockBackDiraction.y = Mathf.Sin(radians);//計算彈回角度
        KnockBackDiraction = KnockBackDiraction.normalized;//正規化

        rb.AddForce(KnockBackDiraction * KnockBackForce, ForceMode.Impulse);//增加彈回力度
        isKnockBack = true;//正在彈回
        knockBackKeeper = KnockDackDuration;//重設彈回時間，並到upate減少至0
    }
}
