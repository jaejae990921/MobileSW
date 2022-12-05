using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") // ź�� ���� ������
        {
            Destroy(gameObject, 0.5f); // 0.5�� �ڿ� �����
        }
        else if (collision.gameObject.tag == "Wall") // �Ѿ� ���� ������
        {
            Destroy(gameObject); // �ٷ� ����
        }
    }
}
