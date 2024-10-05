using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 


public class ValueHandler : MonoBehaviour
{
    public GameObject item;
    public int state;
    public event Action<int,string> OnVariableChanged;

    public int GetState
    {
        get { return state; }
        set
        {
            if (state != value)
            {
                state = value;
                OnVariableChanged?.Invoke(state, "Modified by: " + gameObject.name);  // 传递修改者信息
                Debug.Log($"MyVariable changed to {state} by {gameObject.name}");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        state = item.GetComponent<FrogData>().frog.State;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
