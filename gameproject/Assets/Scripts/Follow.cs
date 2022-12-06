using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset; //고정값 유지를 위해서 
  
    void Update()
    {
        transform.position = target.position + offset;
    }
}
