using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset; //������ ������ ���ؼ� 
  
    void Update()
    {
        transform.position = target.position + offset;
    }
}
