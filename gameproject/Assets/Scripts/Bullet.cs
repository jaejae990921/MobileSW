using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision) //탄피가 충돌이 발생하면 사라지는 코드
    {
        if(collision.gameObject.tag == "Floor") //충돌 로직 작성
        {
            Destroy(gameObject, 3); //3초뒤에 파괴됨.(삭제됨)
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); //딜레이 없이 삭제됨
        }
    }

    void OnTriggerEnter(Collider other) //총알을 위해 OnTriggerEnter() 함수 로직 생성
    {
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject); //총알이 자연스럽게 사라지는 코드
        }
    }
}
