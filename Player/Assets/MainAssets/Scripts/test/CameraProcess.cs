using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraProcess : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform target; // 角色的 Transform
    public float maxYaw = 100f; // 最大水平旋转角度
    public float minYaw = -100f; // 最小水平旋转角度
    public float followSpeed = 0.1f; // 跟随的速度
    bool isOutOfYawLimit = false;
    private bool isFollowing = true; // 是否正在跟随
   
    private CinemachineFramingTransposer transposer; // Cinemachine Transposer 组件

    private void LateUpdate()
    {
        if (Player.rotationDifference > maxYaw && isFollowing)
        {
            isFollowing = false;
            Player.rotationDifference = 0;
            StartCoroutine(FreezeRotate());
        }
    }
    IEnumerator FreezeRotate()
    {
        
        virtualCamera.Follow = null; // 停止跟隨
        virtualCamera.LookAt = null; // 停止看向
        //加入linear跟隨玩家座標的函數********************************
        yield return new WaitForSeconds(1.5f);
        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
        yield return new WaitForSeconds(.5f);
        isFollowing = true;
        yield return null;
    }

    private IEnumerator SmoothFollowTransition(Transform newTarget)
    {
        float duration = 1f; // 平滑過渡的時間
        float elapsed = 0f;

        Vector3 startPosition = virtualCamera.transform.position;
        Vector3 targetPosition = newTarget.position + new Vector3(0.22f, 1.73f, -3.74f) * 1.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 緩慢插值相機位置
            virtualCamera.transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                elapsed / duration
            );
            yield return null;
        }

        // 最後設置相機的 Follow 為新的目標，讓 Cinemachine 接管
    }

    private IEnumerator SmoothLookAtTransition(Transform newTarget)
    {
        float duration = 1f;
        float elapsed = 0f;

        Quaternion startRotation = virtualCamera.transform.rotation;

        // 計算角色背後的目標位置
        Vector3 lookAtPosition = newTarget.position - newTarget.forward; // 使用角色的背後
        Quaternion targetRotation = Quaternion.LookRotation(
            lookAtPosition - virtualCamera.transform.position
        );

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            virtualCamera.transform.rotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                elapsed / duration
            );
            yield return null;
        }

        // 設定 LookAt 為角色的背後
    }

    private void CheckRotationLimits()
    {
        // 获取相机的Transform组件
        Transform camTransform = virtualCamera.transform;
        // 获取相机当前的旋转角度（欧拉角度）
        Vector3 rotation = camTransform.eulerAngles;

        // 将欧拉角度转换到-180 到 180的范围（避免角度跳转问题）
        float yaw = (rotation.y > 180) ? rotation.y - 360 : rotation.y;
        // 检查是否超过水平或俯仰角度限制
        isOutOfYawLimit = yaw < minYaw || yaw > maxYaw;
    }

    bool IsOutOfScreenBounds()
    {
        // 獲取角色的螢幕位置
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);

        // 檢查角色的螢幕位置是否在範圍內
        return screenPosition.x < 0
            || screenPosition.x > Screen.width
            || screenPosition.y < 0
            || screenPosition.y > Screen.height;
    }
}
