using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : StateMachineBehaviour
{
    //playerMovitation
    
   

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        string stateName = animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
        Debug.Log("Start Clip："+stateName);
    }

       override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        
        string stateName = animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
        Debug.Log("End Clip："+stateName);


    }
}
