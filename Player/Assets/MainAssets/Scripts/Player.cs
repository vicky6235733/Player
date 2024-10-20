using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    //
    Rigidbody rb;
    Vector3 MoveDirection; //方向數值
    Vector3 MoveAmount; //位移距離
    Quaternion lastRotation; // 上次的旋轉值
    Quaternion currentRotation; // 當前的旋轉值
    public static float rotationDifference; // 旋轉差值

    //
    LayerMask GroundLayer; //地板圖層
    float GroundRayLength; //地板射線長度
    RaycastHit hitFloor; //射線偵測到的地板
    RaycastHit hitWall; //射線偵測到的牆壁
    LayerMask TriggerLayerMask;

    LayerMask CollisionLayer; //牆壁圖層
    float CollisionRayLength; //牆壁射線長度
    float decelerationFactor; //减速因子

    //
    float MoveSpeed; //走路速度
    float Speed; //衝刺速度
    float RotateSpeed;
    float JumpForce;
    float JumpInterval; // 跳躍間隔
    float LastJumpTime; // 上次跳躍的時間

    //撞牆反饋
    float knockBackAngle; //彈回角度
    float KnockBackForce; //彈回力度
    float KnockDackDuration; //彈回時間長短
    float knockBackKeeper; //彈回後的目前時間

    //
    bool isSlowDown; //是否減速
    bool isKnockBack;
    bool isSprinting; //是否正在衝刺
    bool isOnFloor;
    bool isGettingRotation;
    static bool ColliderEntered;
    bool Sloping;

    //
    Animator anim;
    public Animator Smoke;
    public static bool SpintAnimationRestart; //衝刺動畫是否能重新觸發(在update)

    //
    List<GameObject> targetGroup;

    //
    SkinnedMeshRenderer[] rend; //1 4 8 suger
    public Material[] matBody;
    public Material[] matSuger;
    public static string ColorName;
    string[] AllColor = { "null", "red", "yellow", "blue", "green", "orange", "purple" };

    //
    int ColorCnt;

    void Start()
    {
        //初始化
        anim = GetComponent<Animator>();
        AnimaController(1f);
        lastRotation = transform.rotation;
        rotationDifference = 0;
        //
        rend = new SkinnedMeshRenderer[8];
        for (int i = 0; i < 8; i++)
        {
            rend[i] = transform.GetChild(i).gameObject.GetComponent<SkinnedMeshRenderer>();
        }
        ColorName = "null";
        //
        rb = GetComponent<Rigidbody>();
        GroundLayer = LayerMask.GetMask("Ground");
        GroundRayLength = 0.7f;
        CollisionLayer = LayerMask.GetMask("Collision");
        CollisionRayLength = 2.5f;
        decelerationFactor = 1f;
        TriggerLayerMask = LayerMask.GetMask("TriggerLayer");
        //
        MoveSpeed = 3.5f;
        Speed = MoveSpeed * 2;
        JumpForce = 550f;
        RotateSpeed = 450f;
        JumpInterval = 1f; //跳躍間隔
        LastJumpTime = Time.time - JumpInterval; // 將上次跳躍時間初始化為在間隔之前的時間

        //
        MoveDirection = Vector3.zero;
        MoveAmount = Vector3.zero;
        //
        knockBackAngle = 180f;
        KnockBackForce = 180f;
        KnockDackDuration = 0.5f;
        knockBackKeeper = 0;
        //
        isKnockBack = false;
        isOnFloor = true;
        isSlowDown = false;
        isGettingRotation = false;
        ColliderEntered = false;
        isSprinting = false; //觸發update衝刺
        Sloping = false;

        //
        SpintAnimationRestart = true;
        ColorCnt = 0;
    }

    //爬牆實裝
    //前方有障礙物的時候不能衝刺未完全實踐功能
    //knockback判定要改，他現在會常常被彈回，也會影響到衝刺速度?有時候測又沒用
    //bounce判定未完成
    //掉落到時候改死亡
    //transform.rotation = Quaternion.LookRotation(MoveDirection.normalized);優化

    //有時候跳躍動畫會播放但是他不會跳
    //撞牆硬要往前衝的時候跳不起來


    //注意防止穿到地板下的部分數值有點奇怪
    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        //idle
        Vector3 forward = transform.forward; // 当前朝向
        Vector3 right = transform.right; // 右侧方向
        MoveDirection = forward * ver + right * hor; // 根据输入计算移动方向

        // 规范化移动方向，避免移动速度过快
        if (MoveDirection.magnitude > 1f)
        {
            MoveDirection.Normalize();
        }
       
        if (MoveDirection == Vector3.zero && isOnFloor)
        {
            AnimaController(1f);
            //Smoke.SetBool("Smoke", false);
        }

        //rotate
        Vector3 RotateAmount = hor * Vector3.up * RotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(RotateAmount);
        rb.MoveRotation(rb.rotation * deltaRotation);
        //Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
        //rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime));


        if (hor != 0 && !isGettingRotation)
        {
            lastRotation = transform.rotation;
            isGettingRotation = true;
        }
        if (hor == 0 && isGettingRotation)
        {
            currentRotation = transform.rotation;
            GetRotationDifference();
            isGettingRotation = false;
        }

        //jump
        Vector3 FloorPosition = transform.position + Vector3.up * 0.5f;
        Vector3 ForwardPosition = transform.position + Vector3.forward * 0.5f;

        //偵測地板(用子物件圖層)
        isOnFloor = Physics.Raycast(
            FloorPosition,
            Vector3.down,
            out hitFloor,
            GroundRayLength,
            GroundLayer
        ); //射線偵測下方有沒有地板

        GameObject item = isOnFloor && hitFloor.collider ? hitFloor.collider.gameObject : null;
        if (item != null && item.layer == LayerMask.NameToLayer("Ground"))
        {
            isOnFloor = true;
        }
        else if (item != null && item.transform.childCount > 0)
        {
            foreach (Transform child in item.transform)
            {
                // 這裡你可以判斷子物件的Layer是否符合條件
                if (child.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    isOnFloor = true;
                    break;
                }
                else
                {
                    isOnFloor = false;
                }
            }
        }
        else
        {
            isOnFloor = false;
        }

        if (hitFloor.normal.y < 1f)
        {
            Sloping = true;
        }

        if (
            isOnFloor
            && Input.GetKeyDown(KeyCode.Space)
            && Time.time >= LastJumpTime + JumpInterval
        )
        {
            Jump();
        }

        //防止穿到地板下
        if (!isOnFloor && Mathf.Abs(transform.position.y - hitFloor.point.y) < 0.2f)
        {
            transform.position = new Vector3(
                transform.position.x - 5f,
                transform.position.y + 10f,
                transform.position.z - 5f
            );
            rb.velocity = Vector3.zero;
        }

        //sprint
        if (Input.GetKeyDown(KeyCode.R) && !isSlowDown)
        {
            isSprinting = true; //如果按R就判斷衝刺
            if (SpintAnimationRestart)
            {
                anim.SetBool("SprintStart", true);
                SpintAnimationRestart = false;
            }
        }

        //walk
        if (!Sloping)
        {
            if (MoveDirection != Vector3.zero && !isSprinting)
            {
                Vector3 MoveLength = new Vector3(0, 0, ver);
                rb.MovePosition(rb.position + MoveDirection * MoveSpeed * Time.deltaTime);
                //rb.MovePosition(rb.position + (transform.forward * ver * MoveSpeed * Time.deltaTime));
                AnimaController(2f);
                //Smoke.SetBool("Smoke", true);
            }
        }
        else
        { //在斜坡上走路
            if (MoveDirection.magnitude > 0.1f && !isSprinting)
            {
                Vector3 MoveLength = new Vector3(0, 0, ver);
                rb.MovePosition(
                    rb.position + (transform.forward * ver * MoveSpeed * Time.deltaTime)
                );
                AnimaController(2f);
                //Smoke.SetBool("Smoke", true);
            }
            else
            { //修正斜坡走路動畫一直播的問題
                AnimaController(1f);
                isOnFloor = true;
            }
        }

        Debug.DrawRay(
            transform.position + Vector3.up * .5f,
            Vector3.down * (GroundRayLength + 2f),
            Color.red
        );
        //attack
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetBool("Attack", true);
            Ray ray = new Ray(transform.position + Vector3.up * .5f, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray, GroundRayLength + 1f, TriggerLayerMask);

            foreach (RaycastHit hit in hits)
            {
                Debug.Log(hit.collider.gameObject.name);
                // 检查射线击中的物体是否具有目标标签
                if (hit.collider != null && hit.collider.CompareTag("trigger"))
                {
                    GameObject TriggerObject = hit.collider.gameObject;

                    targetGroup = GetTargetColorPaper<ColorData>(TriggerObject);
                    if (Checkcolor())
                    {
                        Invoke("DrawColor", 1.5f);
                    }
                    else
                    {
                        Debug.Log("Player color wrong");
                    }
                }
            }
        }
        else
        {
            anim.SetBool("Attack", false);
        }

        //slowDown(減速)
        isSlowDown = Physics.Raycast(
            ForwardPosition,
            transform.forward,
            out hitWall,
            CollisionRayLength,
            CollisionLayer
        ); //射線偵測前方有沒有物體
        GameObject wall = isSlowDown && hitWall.collider ? hitWall.collider.gameObject : null;
        if (wall != null && wall.transform.childCount > 0)
        {
            foreach (Transform child in wall.transform)
            {
                // 這裡你可以判斷子物件的Layer是否符合條件
                if (child.gameObject.layer == LayerMask.NameToLayer("Collision"))
                {
                    isSlowDown = true;

                    break;
                }
                else
                {
                    isSlowDown = false;
                }
            }
        }
        else
        {
            isSlowDown = false;
        }
        if (MoveDirection.magnitude > 0.1f && isSlowDown)
        {
            anim.SetBool("Bounce", true);
        }
        else
        {
            anim.SetBool("Bounce", false);
        }

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

    void FixedUpdate() //衝刺用update因為更新太快、衝太快，會有發生更新FPS衝突
    {
        float ver = Input.GetAxis("Vertical");
        if (isSprinting)
        {
            if (MoveDirection != Vector3.zero)
            { //sprint_run(衝刺跑，有加入物理量)
                if (!isSlowDown)
                { //前方有物體就不給他硬衝
                    MoveAmount = transform.forward * ver * Speed * Time.fixedDeltaTime * 1.5f;
                }
            }
            else
            { //sprint(單純衝刺)
                if (!isSlowDown)
                { //前方有物體就不給他硬衝
                    MoveDirection = transform.forward;
                    MoveAmount = MoveDirection * Speed * Time.fixedDeltaTime;
                }
            }
        }

        //walk and sprint
        if (!ColliderEntered)
        {
            StartCoroutine(SprintProcess(MoveAmount));
        }

        //slowDown
        //SlowDown();
    }

    void Jump()
    {
        anim.SetTrigger("jump");
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        LastJumpTime = Time.time; // 更新上次跳躍的時間
    }

    void GetRotationDifference()
    {
        rotationDifference = 0;
        rotationDifference += Quaternion.Angle(lastRotation, currentRotation);
        rotationDifference =
            (rotationDifference > 180) ? rotationDifference - 360 : rotationDifference;
        lastRotation = currentRotation;
    }

    void SlowDown()
    { //如果前方有物體在靠近，就讓玩家減速(避免玩家硬要撞穿模)
        rb.velocity = new Vector3(
            Mathf.Lerp(rb.velocity.x, 0, decelerationFactor * Time.fixedDeltaTime),
            rb.velocity.y,
            Mathf.Lerp(rb.velocity.z, 0, decelerationFactor * Time.fixedDeltaTime)
        );
    }

    void StopMove() //強制停下衝刺的物理慣性
    {
        MoveAmount = Vector3.zero;
        isSprinting = false;
    }

    IEnumerator SprintProcess(Vector3 Amount)
    {
        yield return new WaitForSeconds(0.3f);
        while (Amount.magnitude > 0.1f)
        {
            if (!anim.GetBool("SprintStart"))
            {
                rb.MovePosition(rb.position + Amount);
                Invoke("StopMove", 0.8f);
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    //
    void AnimaController(float item)
    {
        anim.SetFloat("Action", item);
    }

    //
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.transform.childCount > 0)
        {
            foreach (Transform child in other.gameObject.transform)
            {
                // 這裡你可以判斷子物件的Layer是否符合條件
                if (child.gameObject.layer == LayerMask.NameToLayer("Collision"))
                {
                    //MoveAmount = Vector3.zero; //停止移動
                    if (rb.velocity.magnitude > 0.1f)
                    {
                        rb.AddForce(-rb.velocity.normalized); //給予反向力讓玩家停止
                    }
                    else
                    { // 避免來回震動，將速度限制在接近零時直接設為零
                        rb.velocity = Vector3.zero;
                    }
                    ColliderEntered = true;

                    KnockBack(other); //用other偵測前方物體位置，方便計算反彈角度
                }
            }
        }

        if (other.gameObject.tag == "Frog")
        {
            ColorCnt++;
            ColliderEntered = true;
            KnockBack(other); //用other偵測前方物體位置，方便計算反彈角度
            if (ColorCnt == 1)
            {
                ChangeMaterial(Array.IndexOf(AllColor, other.gameObject.GetComponent<FrogData>().frog.Color));
                ColorName = other.gameObject.GetComponent<FrogData>().frog.Color;
            }
            else if (ColorCnt == 2)
            {
                if (ColorName == other.gameObject.GetComponent<FrogData>().frog.Color)
                {
                    ColorCnt = 1;
                }
                else
                {
                    if (ColorName == "red")
                    {
                        if (other.gameObject.GetComponent<FrogData>().frog.Color == "yellow")
                        {
                            ColorName = "orange";
                        }
                        else if (other.gameObject.GetComponent<FrogData>().frog.Color == "blue")
                        {
                            ColorName = "purple";
                        }
                    }
                    if (ColorName == "yellow")
                    {
                        if (other.gameObject.GetComponent<FrogData>().frog.Color == "red")
                        {
                            ColorName = "orange";
                        }
                        else if (other.gameObject.GetComponent<FrogData>().frog.Color == "blue")
                        {
                            ColorName = "green";
                        }
                    }
                    if (ColorName == "blue")
                    {
                        if (other.gameObject.GetComponent<FrogData>().frog.Color == "red")
                        {
                            ColorName = "purple";
                        }
                        else if (other.gameObject.GetComponent<FrogData>().frog.Color == "yellow")
                        {
                            ColorName = "green";
                        }
                    }
                    ChangeMaterial(Array.IndexOf(AllColor, ColorName));
                }
            }
            else
            {
                Debug.Log("Color Reset time");
            }
        }
        if (other.gameObject.tag == "BigFrog")
        {
            ColliderEntered = true;
            KnockBack(other); //用other偵測前方物體位置，方便計算反彈角度
            ChangeMaterial(0);
            ColorName = "null";
            ColorCnt = 0;
        }
    }

    void KnockBack(Collision target) //彈回
    {
        Vector3 KnockBackDirection = (transform.position - target.transform.position).normalized;

        float radians = knockBackAngle * Mathf.Deg2Rad;
        KnockBackDirection.y = Mathf.Sin(radians); // 計算彈回角度

        // 增加向上的力，創造拋物線效果
        KnockBackDirection.y += 0.5f; // 調整這個值來改變拋物線的高度

        KnockBackDirection = KnockBackDirection.normalized; // 正規化

        rb.AddForce(KnockBackDirection * KnockBackForce, ForceMode.Impulse); // 增加彈回力度
        isKnockBack = true; // 正在彈回
        knockBackKeeper = KnockDackDuration; // 重設彈回時間，並在 Update 減少至 0
    }

    void DrawColor()
    {
        foreach (GameObject item in targetGroup)
        {
            item.SetActive(true);
            item.GetComponent<ColorData>().colorPaper.Activity = true;
        }
    }

    void ChangeMaterial(int num)
    {
        if (rend != null)
        {
            // 將修改後的材質陣列重新賦值給 SkinnedMeshRenderer
            for (int i = 0; i < rend.Length; i++)
            {
                // 複製現有的材質陣列
                Material[] mats = rend[i].materials;

                //修改
                if (mats.Length > 0)
                {
                    if (i != 0 && i != 3 && i != 7)
                    {
                        mats[0] = matBody[num];
                    }
                    else
                    {
                        mats[0] = matSuger[num];
                    }
                }

                rend[i].materials = mats;
            }
        }
        /*rend[0].material = matSuger[num]; //1
        rend[1].material = matBody[num];
        rend[2].material = matBody[num];
        rend[3].material = matSuger[num]; //4
        rend[4].material = matBody[num];
        rend[5].material = matBody[num];
        rend[6].material = matBody[num];
        rend[7].material = matSuger[num]; //8*/
    }

    bool Checkcolor()
    {
        if (targetGroup != null)
        {
            int CheckID = targetGroup[0].GetComponent<ColorData>().colorPaper.ID;
            PaperItemStruct PaperData = FindObjectsOfType<PaperData>()
                .Where(PaperData => PaperData.paper.ID == CheckID)
                .Select(PaperData => PaperData.paper)
                .FirstOrDefault();

            if (
                PaperData.State != "stay"
                && targetGroup[0].GetComponent<ColorData>().colorPaper.Activity == false
            )
            {
                if (ColorName == targetGroup[0].GetComponent<ColorData>().colorPaper.Color)
                {
                    targetGroup[0].GetComponent<ColorData>().colorPaper.Activity = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.Log("Paper state wrong,or Color paper activity wrong");
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public List<GameObject> GetTargetColorPaper<T>(GameObject trigger)
        where T : Component //尋找具有特定組件的子物件
    {
        List<GameObject> childrenWithComponent = new List<GameObject>();

        for (int i = 0; i < trigger.transform.childCount; i++)
        {
            Transform child = trigger.transform.GetChild(i);

            // 检查子物体是否具有 T 类型的组件
            if (child.GetComponent<T>() != null)
            {
                childrenWithComponent.Add(child.gameObject);
            }
        }

        return childrenWithComponent;
    }
}
