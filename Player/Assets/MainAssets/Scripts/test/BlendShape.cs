using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlendShape : MonoBehaviour
{
    // SkinnedMeshRenderer 用于访问模型的 BlendShape
    private SkinnedMeshRenderer skinnedMeshRenderer;
    // 初始化，找到模型上的 SkinnedMeshRenderer
    void Start()
    {
        // 获取附加在同一对象上的 SkinnedMeshRenderer 组件
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(SetBlendShapeWeight(0, 20f)); 
    }
  
    // 设置 BlendShape 的权重
    IEnumerator SetBlendShapeWeight(int index, float weight)
    {
        // 设置索引为 index 的 BlendShape 权重
        while(weight<100){
            skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
            weight+=10;
            yield return new WaitForSeconds(0.8f);
        }
        yield break;
       
    }
}
