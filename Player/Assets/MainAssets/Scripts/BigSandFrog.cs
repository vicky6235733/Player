using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigSandFrog : MonoBehaviour
{
    int[] PaperLines = { 18, 17, 16, 15, 14, 19, 27, 23, 24, 20, 26, 28, 3, 2, 7, 1, 8, 9, 12, 13 }; //撕貼紙順序
    public AnimationClip clip;
    float runClipLength; // 獲取 Run 動畫的長度
    public Transform[] StickeTransforms = new Transform[20];
    public FrogDataStruct[,] FrogList; //二維陣列
    int road;
    public GameObject[] Frogs = new GameObject[9];
    public GameObject ParentFrog;
    int[] cnt = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    bool StartCheck; //開始撕貼紙

    Animator[] anim;
    int isFuncRunning = 0;
    float WaitingSecond = 5f;

    void Awake()
    {
        for (int i = 0; i < Frogs.Length; i++)
        {
            Frogs[i].GetComponent<FrogData>().frog.State = 2; //初始狀態設定不撕貼紙
        }
        //初始化陣列
        FrogList = new FrogDataStruct[PaperLines.Length, 2]; //new實例不能改到原始數值，純粹用於順序
        for (int n = 0; n < FrogList.GetLength(0); n++)
        {
            for (int m = 0; m < FrogList.GetLength(1); m++)
            {
                FrogList[n, m] = new FrogDataStruct(false);
            }
        }
    }

    void Start()
    {
        StartCheck = true;
        if (clip != null)
        {
            runClipLength = clip.length;
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

        for (int i = 0; i < StickeTransforms.Length; i++)
        {
            StickeTransforms[i] = FindObjectsOfType<PaperData>()
                .Where(PaperData => PaperData.paper.ID == PaperLines[i] + 30)
                .Select(PaperData => PaperData.paper.transform)
                .FirstOrDefault();
        }

        anim = new Animator[Frogs.Length];
        for (int i = 0; i < Frogs.Length; i++)
        {
            anim[i] = Frogs[i].GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (StartCheck)
        {
            StartCoroutine(StickerStatement()); //轉換到撕貼紙模式
            StartCheck = false;
        }
        if (isFuncRunning==0)
        {
            StartCoroutine(StateMotion());
        }
    }
    
    IEnumerator StateMotion() //目前為了不和貼紙衝突使撕完貼紙才改NPC 然後NPC不會一直跳
    {
        yield return new WaitForSeconds(WaitingSecond);
        WaitingSecond = 0;
        for (int i = 0; i < Frogs.Length; i++)
        {
            isFuncRunning++;
            if (Frogs[i].GetComponent<FrogData>().frog.State == 2)
            {
                if (!Frogs[i].GetComponent<FrogData>().frog.isStay)
                {
                    
                    StartCoroutine(CheckFrogState_NPC(i));
                }else{
                    continue;
                }
            }
            else
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(.5f);
        }
        
        yield break;
    }

    IEnumerator StickerStatement()
    {
        List<int> num = new List<int>();
        for (int i = 0; i < FrogList.GetLength(0); i++) //開始撕貼紙
        {
            for (int j = 0; j < FrogList.GetLength(1); j++)
            {
                if (FrogList[i, j].isInLine == true) //如果struct不為空值
                {
                    FrogList[i, j].State = 1; //更換狀態(不會更改到原本的data)

                    num.Add(
                        FindObjectsOfType<FrogData>()
                            .Where(FrogData => FrogData.frog.Number == FrogList[i, j].Number)
                            .Select(FrogData => FrogData.frog.Number)
                            .FirstOrDefault() - 1
                    ); //青蛙編號從1開始，物件陣列從0開始

                    if (FrogList[i, 1].isInLine == true) //如果有多隻青蛙負責同一張貼紙
                    {
                        if (j + 1 == FrogList.GetLength(1)) //如果已經到所有青蛙結尾
                        {
                            //一張貼紙的所有青蛙結束state更改
                            yield return StartCoroutine(CheckFrogState(num, i));
                        }
                        else
                        {
                            //先等全部都變state=1再疊青蛙
                            yield return null;
                            continue;
                        }
                    }
                    else
                    {
                        yield return StartCoroutine(CheckFrogState(num, i)); //呼叫該編號青蛙，等待他跳完才能繼續下一隻
                    }
                    FrogList[i, j].State = 0; //更換狀態(不會更改到原本的data)
                }
                ChangeToNPC(num, i); //撕完貼紙切換成NPC
                num.Clear();
            }
        }

        yield break;
    }

    void ChangeToNPC(List<int> num, int cnt) //撕完貼紙後切換為NPC
    {
        foreach (var i in num)
        {
            Frogs[i].GetComponent<FrogData>().frog.State = 2; //切換成NPC狀態
            Frogs[i].GetComponent<FrogData>().frog.isStay = false;
        }

        for (int j = 0; j < FrogList.GetLength(1); j++) //同步frogList狀態
        {
            if (FrogList[cnt, j].isInLine == true)
            {
                FrogList[cnt, j].State = 2; //NPC狀態
            }
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    IEnumerator CheckFrogState_NPC(int num) //確認青蛙是否回到原點(NPC)
    {
        while (true) //如果開頭跳完已經輪到青蛙撕貼紙，但青蛙還沒跳完，就等他isJumping回到原點才判斷狀態
        {
            if (Frogs[num].GetComponent<FrogData>().frog.isJumping == false && !Frogs[num].GetComponent<FrogData>().frog.isStay) //回到原點才跳
            {
                yield return StartCoroutine(NPCJump(num));
                yield return null;
                break;
            }
            else
            {
                yield return null;
            }
        }

        yield break;
    }

    IEnumerator CheckFrogState(List<int> num, int currentPaper) //確認青蛙是否回到原點(sticker)
    {
        foreach (var i in num)
        {
            Frogs[i].GetComponent<FrogData>().frog.State = 1;
            Frogs[i].GetComponent<FrogData>().frog.isStay = true;
            
        }
        if (num.Count <= 1) //只有一隻青蛙
        {
            while (true) //如果開頭跳完已經輪到青蛙撕貼紙，但青蛙還沒跳完，就等他isJumping回到原點才判斷狀態
            {
                if (Frogs[num[0]].GetComponent<FrogData>().frog.isJumping == false) //回到原點才判斷狀態
                {
                    yield return StartCoroutine(StickerJump(num[0], currentPaper));
                    yield return null;
                    break;
                }
                else
                {
                    yield return null;
                }
            }
        }
        else
        { //有多隻青蛙
            while (true)
            {
                int cnt = 0;
                foreach (var n in num)
                {
                    if (Frogs[n].GetComponent<FrogData>().frog.isJumping == false)
                    {
                        cnt++;
                    }
                }
                if (cnt == num.Count)
                {
                    yield return StartCoroutine(DoubleFrogStickerJump(num, currentPaper));
                    yield return null;
                    break;
                }
                else
                {
                    yield return null;
                }
            }
        }
        yield break;
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    IEnumerator NPCJump(int num) //青蛙預設兩條路
    {
        if (cnt[num] == 0)
        {
            road = UnityEngine.Random.Range(0, 1);
        }
        GameObject currentFrog = Frogs[num];
        FrogDataStruct currentFrogData = currentFrog.GetComponent<FrogData>().frog;

        if (road == 0)
        {
            StartCoroutine(JumpForoRoad(currentFrogData.TargetLine, currentFrog, 0, num));
            yield return null;
        }
        if (road == 1)
        {
            StartCoroutine(JumpForoRoad(currentFrogData.TargetLine02, currentFrog, 0, num));
            yield return null;
        }

        if (cnt[num] >= 4)
        {
            cnt[num] = 0;
            
           
        }
        else
        {
            cnt[num]++;
        }
        yield break;
    }

    IEnumerator StickerJump(int num, int currentPaperNum) //用貼紙編號決定青蛙
    {
        GameObject currentFrog = Frogs[num];
        currentFrog.GetComponent<FrogData>().frog.CurrentTargetPaper = PaperLines[currentPaperNum];

        Vector3 CurrentFrogTrans = currentFrog.transform.position;
        currentFrog.GetComponent<FrogData>().frog.isJumping = true;
        currentFrog.GetComponent<FrogData>().isStateLocked = true;

        yield return StartCoroutine(
            JumpToTarget(
                currentFrog,
                currentFrog.transform.position,
                StickeTransforms[currentPaperNum].position,
                num
            )
        ); //跳過去貼紙
        yield return new WaitForSeconds(.2f);
        yield return StartCoroutine(
            JumpToTarget(
                currentFrog,
                StickeTransforms[currentPaperNum].position,
                CurrentFrogTrans,
                num
            )
        ); //跳回來
        currentFrog.GetComponent<FrogData>().frog.isJumping = false;
    }

    IEnumerator DoubleFrogStickerJump(List<int> num, int currentPaperNum) //動畫還沒改成分別青蛙的anim[num] 暫時用1代替
    {
        GameObject currentFrog = Frogs[num[0]];
        GameObject currentFrog02 = Frogs[num[1]];
        currentFrog.GetComponent<FrogData>().frog.CurrentTargetPaper = PaperLines[currentPaperNum];
        currentFrog02.GetComponent<FrogData>().frog.CurrentTargetPaper = PaperLines[
            currentPaperNum
        ];

        //

        Vector3 FrogTrans = currentFrog.transform.position;
        Vector3 FrogTrans02 = currentFrog02.transform.position;
        //決定哪隻青蛙在下面(距離較遠的跳到距離較近的，再一起撕貼紙)
        int BottomFrogNum =
            Vector3.Distance(
                currentFrog.transform.position,
                StickeTransforms[currentPaperNum].position
            )
            < Vector3.Distance(
                currentFrog02.transform.position,
                StickeTransforms[currentPaperNum].position
            )
                ? 0
                : 1;
        currentFrog.GetComponent<FrogData>().frog.isJumping = true;
        currentFrog02.GetComponent<FrogData>().frog.isJumping = true;

        if (BottomFrogNum == 0)
        { //0 is bottom
            currentFrog.GetComponent<FrogData>().isStateLocked = true;

            yield return StartCoroutine(
                JumpToTarget(currentFrog02, FrogTrans02, FrogTrans + new Vector3(0, .4f, 0), num[1])
            );
            //currentFrog02.GetComponent<Rigidbody>().isKinematic = true;
            currentFrog02.transform.SetParent(currentFrog.transform);
            yield return StartCoroutine(
                JumpToTarget(
                    currentFrog,
                    FrogTrans,
                    StickeTransforms[currentPaperNum].position,
                    num[0]
                )
            ); //跳過去貼紙
            yield return new WaitForSeconds(.2f);
            yield return StartCoroutine(
                JumpToTarget(
                    currentFrog,
                    StickeTransforms[currentPaperNum].position,
                    FrogTrans,
                    num[0]
                )
            ); //跳回來
            yield return StartCoroutine(
                JumpToTarget(currentFrog02, currentFrog02.transform.position, FrogTrans02, num[1])
            ); //跳回來
            currentFrog02.transform.SetParent(ParentFrog.transform);
            //currentFrog02.GetComponent<Rigidbody>().isKinematic = false;
        }
        else if (BottomFrogNum == 1)
        { //1 is bottom
            currentFrog02.GetComponent<FrogData>().isStateLocked = true;
            yield return StartCoroutine(
                JumpToTarget(currentFrog, FrogTrans, FrogTrans02 + new Vector3(0, .4f, 0), num[0])
            );
            //currentFrog.GetComponent<Rigidbody>().isKinematic = true;
            currentFrog.transform.SetParent(currentFrog02.transform);
            yield return StartCoroutine(
                JumpToTarget(
                    currentFrog02,
                    FrogTrans02,
                    StickeTransforms[currentPaperNum].position,
                    num[1]
                )
            ); //跳過去貼紙
            yield return new WaitForSeconds(.2f);
            yield return StartCoroutine(
                JumpToTarget(
                    currentFrog02,
                    StickeTransforms[currentPaperNum].position,
                    FrogTrans02,
                    num[1]
                )
            ); //跳回來
            yield return StartCoroutine(
                JumpToTarget(currentFrog, currentFrog.transform.position, FrogTrans, num[0])
            ); //跳回來
            currentFrog.transform.SetParent(ParentFrog.transform);
            //currentFrog.GetComponent<Rigidbody>().isKinematic = false;
        }
        currentFrog.GetComponent<FrogData>().frog.isJumping = false;
        currentFrog02.GetComponent<FrogData>().frog.isJumping = false;
    }

    IEnumerator JumpForoRoad(Transform[] Point, GameObject frog, int current, int num)
    {
        // 狀態設定
        float MoveSpeed = 4f; // 移動速度
        float JumpHeight = 1.5f;
        int totalPoints = Point.Length; // 獲取點的總數

        while (true)
        {
            // 設置起點和終點
            Vector3 StartPosition = Point[current].position; // 在一次跳躍中的起點
            Vector3 TargetPosition = Point[(current + 1) % totalPoints].position; // 在一次跳躍中的終點

            frog.GetComponent<FrogData>().frog.isJumping = true;

            // 旋轉處理
            Vector3 directionToTarget = (TargetPosition - StartPosition).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            float rotationSpeed = 200f; // 調整旋轉速度

            float MoveDistance = Vector3.Distance(StartPosition, TargetPosition); // 距離長度
            float JumpDuration = MoveDistance / MoveSpeed; // 移動時間
            if (MoveDistance <= 3)
            {
                JumpDuration *= 2f;
            }
            if (MoveDistance >= 5)
            {
                JumpDuration /= 1.7f;
            }
            JumpDuration /= 2f;
            float now = 0;

            anim[num].speed = (runClipLength / JumpDuration) * 1.2f;
            anim[num].SetTrigger("BFrogJump");
            yield return new WaitForSeconds(0.4f);
            // 跳躍處理
            while (now < JumpDuration)
            {
                now += Time.deltaTime;
                float t = now / JumpDuration; // 當前時間段數值

                // 處理旋轉
                frog.transform.rotation = Quaternion.RotateTowards(
                    frog.transform.rotation,
                    lookRotation,
                    rotationSpeed * Time.deltaTime
                );
                frog.transform.eulerAngles = new Vector3(0, frog.transform.eulerAngles.y, 0); // 校正XZ旋轉

                // 改變曲線使起跳快而下降慢
                float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f); // 起跳快，下降慢
                float CurrentHeight = Mathf.Sin(t * Mathf.PI) * JumpHeight; // 拋物線高度

                // 更新位置
                frog.transform.position =
                    Vector3.Lerp(StartPosition, TargetPosition, curvedT)
                    + new Vector3(0, CurrentHeight, 0);

                yield return null;
            }

            // 設置目標位置
            frog.transform.position = TargetPosition;

            yield return new WaitForSeconds(.8f); // 跳躍之間的等待時間

            // 更換目標點
            current = (current + 1) % totalPoints;

            // 停止條件
            if (current == 0)
            {
                frog.GetComponent<FrogData>().frog.isJumping = false;
                if(isFuncRunning>8){
                isFuncRunning=0;
            }

                yield break; // 跳出循環結束協程
            }
        }
    }

    IEnumerator JumpToTarget(
        GameObject frog,
        Vector3 StartPosition,
        Vector3 TargetPosition,
        int num
    )
    {
        // 狀態設定
        float MoveSpeed = 4f; // 移動速度
        float JumpHeight = 1.5f;

        Vector3 directionToTarget = (TargetPosition - StartPosition).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        float rotationSpeed = 200f; // 調整旋轉速度

        float MoveDistance = Vector3.Distance(frog.transform.position, TargetPosition); //距離長度
        float JumpDuration = MoveDistance / MoveSpeed; //移動時間
        if (MoveDistance <= 3)
        {
            JumpDuration *= 2f;
        }
        if (MoveDistance >= 5)
        {
            JumpDuration /= 1.7f;
        }
        JumpDuration /= 2f;
        float now = 0;

        anim[num].speed = (runClipLength / JumpDuration) * 1.2f;
        anim[num].SetTrigger("BFrogJump");
        yield return new WaitForSeconds(.4f);
        while (now < JumpDuration)
        {
            now += Time.deltaTime;
            float t = now / JumpDuration; //當前時間段數值

            // 處理旋轉
            frog.transform.rotation = Quaternion.RotateTowards(
                frog.transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
            frog.transform.eulerAngles = new Vector3(0, frog.transform.eulerAngles.y, 0); // 校正XZ旋轉

            // 改變曲線使起跳快而下降慢
            float curvedT = Mathf.Sin(t * Mathf.PI * 0.5f); // 起跳快，下降慢
            float CurrentHeight = Mathf.Sin(t * Mathf.PI) * JumpHeight; // 拋物線高度

            // 更新位置
            frog.transform.position =
                Vector3.Lerp(StartPosition, TargetPosition, curvedT)
                + new Vector3(0, CurrentHeight, 0);

            yield return null;
        }
        frog.transform.position = TargetPosition;
        frog.GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield break;
    }
}
