using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; // 열거형 타입
    public Type type; // 무기 타입
    public int value; // 해머 0, 핸드건 1, 서브머신건 2


    Rigidbody rigid; //아이템 물리 충돌을 담당하는 콜라이더와 충돌하여 문제 발생 fix
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 컴포넌트 값으로 물리효과 초기화
        sphereCollider = GetComponent<SphereCollider>(); // 컴포넌트 값으로 콜라이더 초기화
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); // 무기 회전
    }

    void OnCollisionEnter(Collision collision) // 물리효과 변경
    {
        if(collision.gameObject.tag == "Floor") // 바닥에 닿음 조건
        {
            rigid.isKinematic = true; //rigidbody는 외부 물리 효과에 의해 움직이지 못하게함.
            sphereCollider.enabled = false;
        }
    }
}
