using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splacescreen : MonoBehaviour
{
    public static Splacescreen instance;
    void Start()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    
}
