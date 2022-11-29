using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision) //ź�ǰ� �浹�� �߻��ϸ� ������� �ڵ�
    {
        if(collision.gameObject.tag == "Floor") //�浹 ���� �ۼ�
        {
            Destroy(gameObject, 3); //3�ʵڿ� �ı���.(������)
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); //������ ���� ������
        }
    }

    void OnTriggerEnter(Collider other) //�Ѿ��� ���� OnTriggerEnter() �Լ� ���� ����
    {
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject); //�Ѿ��� �ڿ������� ������� �ڵ�
        }
    }
}
