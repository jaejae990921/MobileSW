using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; // enum 열거형 타입, { } 안에 데이터를 열거하듯이 작성
    public Type type; // 무기 타입
    public int value; // 해머 0, 총 1

    Rigidbody rigid; // 물리효과
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 컴포넌트 값으로 리지드바디 초기화
        sphereCollider = GetComponent<SphereCollider>(); // 컴포넌트 값으로 콜라이더 초기화
    }

    void Update() {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); // 무기 회전
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }   
    }

}
