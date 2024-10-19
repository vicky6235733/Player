using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    
    void Start()
    {
        
    }

// Update is called once per frame  
    void Update()
    {
        //rb.rotation = Quaternion.Euler(0,0.05f,0);
        transform.Rotate(0.1f,0,0,Space.Self);
       
    }
   
}
