using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
 
    public bool isMelee;
    public bool isRock;
    public bool isEnemy;
    public int damage;

  

    void OnCollisionEnter(Collision collision) //탄피가 충돌이 발생하면 사라지는 코드
    {
        if (isEnemy) return;
        if (collision.gameObject.tag == "Enemy") return;
        if(!isRock && collision.gameObject.tag == "Floor") //충돌 로직 작성
        {
            
            Destroy(gameObject, 0.5f); //3초뒤에 파괴됨.(삭제됨)
        }
        
    }

    void OnTriggerEnter(Collider other) //총알을 위해 OnTriggerEnter() 함수 로직 생성
    {
        if (isEnemy) return;

        if (!isMelee && other.gameObject.tag == "Wall" || !isRock && !isMelee && other.gameObject.tag == "Floor") //근접공격 범위가 파괴되지 않도록 조건추가
        {
            Destroy(gameObject); //총알이 자연스럽게 사라지는 코드
        }
    }

    
}
