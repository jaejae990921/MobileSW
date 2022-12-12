using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 7;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") // ź�� ���� ������
        {
            Destroy(gameObject, 0.5f); // 0.5�� �ڿ� �����
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Floor") // �Ѿ� ���� ������
        {
            Destroy(gameObject); // �ٷ� ����
        }
    }
}