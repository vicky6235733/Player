using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;
    public float timer;
    public float missTime;
    public float resetTime;
    public bool isTouch,isMiss;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTouch)
        {
            timer += Time.deltaTime;
            if(timer >= missTime)
            {
                timer = 0;
                isTouch = false;
                isMiss = true;
                target.SetActive(false);
            }
        }
        if (isMiss)
        {
            timer += Time.deltaTime;
            if(timer >= resetTime)
            {
                timer = 0;
                isMiss = false;
                target.SetActive(true);
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isTouch = true;
        }
    }
}
