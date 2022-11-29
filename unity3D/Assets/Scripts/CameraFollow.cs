using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 따라갈 목표
    public Vector3 offset; // 위치

    void Update()
    {
        transform.position = target.position + offset; // 타겟에서 고정값을 더한 값이 위치가 됨
    }
}
