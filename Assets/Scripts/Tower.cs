using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public Stack<Ring> Ringstack = new Stack<Ring>();
    public GameManager gm;
    public Transform[] position;

    
    private void OnMouseDown()
    {
        gm.Towerclick(this);   
        
    }

}
