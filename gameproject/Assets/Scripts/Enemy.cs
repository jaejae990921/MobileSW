using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int maxHealth; //체력과 컴포넌스를 담을 변수 선언
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat; //물체 색

    void Awake() //초기화
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other) //날아오는 총알, 해머
    {
        if(other.tag == "Melee") //근접무기 공격 맞을때
        {
            Weapon weapon = other.GetComponent<Weapon>(); //충돌 상대의 스크립트를 가져와 damage값을 체력에 적용
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet") //원거리 공격 맞을때
        {
            Bullet bullet = other.GetComponent<Bullet>(); //충돌 상대의 스크립트를 가져와 damage값을 체력에 적용
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알의 경우 적과 닿았을때 삭제 되도록 코드


            StartCoroutine(OnDamage(reactVec));
        }
        
    }

    IEnumerator OnDamage(Vector3 reactVec) //피격로직 (로직을 담을 코르틴 생성
    {
        mat.color = Color.red; //색깔 입히는 코드 ★
        yield return new WaitForSeconds(0.1f); //시간 정하는 코드

        if(curHealth > 0)
        {
            mat.color = Color.white; //아직죽지 않았을시 색상 하얀색
        }
        else
        {
            mat.color = Color.gray; //죽으면 회색으로 변경
            gameObject.layer = 14; //레이어 그대로 14번 (더이상 물리 충돌 하지 않고)

            reactVec = reactVec.normalized; //리엑션을 위해 vector을 선언
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //함수로 넉백 구현하기 (뒷쪽으로 힘이 가해진다)

            Destroy(gameObject, 4); //죽었을시 4초뒤 파괴됨 
        }
    }

}
