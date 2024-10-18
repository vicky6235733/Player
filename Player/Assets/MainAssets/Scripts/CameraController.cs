using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // 将你的 FreeLook 相机拖到这个变量上
    public float dragSpeed; // 拖曳速度
    public float smoothTime; // 平滑时间
    public float mouseMoveThreshold = 1f; // 滑鼠位移阈值

    private bool isDragging = false; // 用于判断是否在拖曳
    private float previousMouseX; // 前一帧的滑鼠X位置
    private float rotationVelocity; // 用于平滑计算的速度
    private float targetRotation; // 目标旋转

    public float stopRotationThreshold = 0.1f; // 定义停止旋转的阈值

    void Start()
    {
        targetRotation = freeLookCamera.m_XAxis.Value; // 设置初始目标旋转
    }

    void Update()
    {
        // 检查鼠标按钮是否被按下或放开
        if (Input.GetMouseButtonDown(0)) // 左键按下
        {
            isDragging = true; // 开始拖曳
            previousMouseX = Input.mousePosition.x; // 记录当前鼠标位置
        }

        if (Input.GetMouseButtonUp(0)) // 左键放开
        {
            isDragging = false; // 停止拖曳
            // 将当前的旋转作为目标旋转
            targetRotation = freeLookCamera.m_XAxis.Value; 
        }

        // 如果正在拖曳，检查滑鼠位移是否超过阈值
        if (isDragging)
        {
            float deltaX = Input.mousePosition.x - previousMouseX; // 计算滑鼠移动的距离

            // 只有在滑鼠位移超过阈值时才更新目标旋转
            if (Mathf.Abs(deltaX) > mouseMoveThreshold)
            {
                float rotationAmount = deltaX * dragSpeed * Time.deltaTime; // 计算相机旋转的量
                targetRotation += rotationAmount; // 更新目标旋转
            }

            previousMouseX = Input.mousePosition.x; // 更新前一帧的鼠标位置
        }

        // 使用 SmoothDamp 进行平滑过渡
        float currentRotation = freeLookCamera.m_XAxis.Value; // 当前旋转
        float newRotation = Mathf.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

        // 更新相机的旋转
        freeLookCamera.m_XAxis.Value = newRotation;

        // 检查是否接近目标旋转
        if (Mathf.Abs(newRotation - targetRotation) < stopRotationThreshold)
        {
            // 停止拖曳，保持在当前目标旋转
            targetRotation = newRotation;
        }
    }
}
