using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring : MonoBehaviour
{
     bool isReset;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        isReset = false;
        anim = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if(isReset){
            Invoke("Reset", 2f);
            isReset = false;
        }
    }
    void Reset(){
        anim.SetTrigger("PaperReset");
        
    }
}
