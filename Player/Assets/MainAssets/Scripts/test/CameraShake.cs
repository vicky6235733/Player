using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    Vector3 OriginalPosition;

    void Start()
    {
        OriginalPosition = transform.position;
    }
    public IEnumerator Shake(float Duration, float Magnitude)//時間 幅度
    {
        
        float now = 0.0f;

        while (now < Duration)
        {
            float x = OriginalPosition.x + Random.Range(-1f, 1f) * Magnitude;
            float y = OriginalPosition.y + Random.Range(-1f, 2f) * Magnitude;

            transform.position = new Vector3(x, y, OriginalPosition.z);

            now += Time.deltaTime;

            yield return null;
        }

        transform.position = OriginalPosition;
       
    }
}
