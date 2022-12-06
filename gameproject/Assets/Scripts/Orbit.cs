using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet; //������



    void Start()
    {
        offSet = transform.position - target.position; //����ź ��ġ���� Ÿ����ġ�� ��
    }

 
    void Update()
    {
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); //Ÿ�� ������ ȸ���ϴ� �Լ�
        offSet = transform.position - target.position;
    }
}
