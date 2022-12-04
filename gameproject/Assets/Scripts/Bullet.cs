using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision collision) //ź�ǰ� �浹�� �߻��ϸ� ������� �ڵ�
    {
        if(!isRock && collision.gameObject.tag == "Floor") //�浹 ���� �ۼ�
        {
            Destroy(gameObject, 3); //3�ʵڿ� �ı���.(������)
        }
        
    }

    void OnTriggerEnter(Collider other) //�Ѿ��� ���� OnTriggerEnter() �Լ� ���� ����
    {
        if (!isMelee && other.gameObject.tag == "Wall") //�������� ������ �ı����� �ʵ��� �����߰�
        {
            Destroy(gameObject); //�Ѿ��� �ڿ������� ������� �ڵ�
        }
    }
}
