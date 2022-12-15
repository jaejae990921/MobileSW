using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
 
    public bool isMelee;
    public bool isRock;
    public bool isEnemy;
    public int damage;

  

    void OnCollisionEnter(Collision collision) //ź�ǰ� �浹�� �߻��ϸ� ������� �ڵ�
    {
        if (isEnemy) return;
        if (collision.gameObject.tag == "Enemy") return;
        if(!isRock && collision.gameObject.tag == "Floor") //�浹 ���� �ۼ�
        {
            
            Destroy(gameObject, 0.5f); //3�ʵڿ� �ı���.(������)
        }
        
    }

    void OnTriggerEnter(Collider other) //�Ѿ��� ���� OnTriggerEnter() �Լ� ���� ����
    {
        if (isEnemy) return;

        if (!isMelee && other.gameObject.tag == "Wall" || !isRock && !isMelee && other.gameObject.tag == "Floor") //�������� ������ �ı����� �ʵ��� �����߰�
        {
            Destroy(gameObject); //�Ѿ��� �ڿ������� ������� �ڵ�
        }
    }

    
}
