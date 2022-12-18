using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour // 수류탄 기능 관련 스크립트
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet; //고정값



    void Start()
    {
        offSet = transform.position - target.position; //수류탄 위치에서 타겟위치를 뺌
    }

 
    void Update()
    {
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); //타겟 주위를 회전하는 함수
        offSet = transform.position - target.position;
    }
}
