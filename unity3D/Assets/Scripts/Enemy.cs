using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Nav 관련 클래스 사용

public class Enemy : MonoBehaviour
{
    public int MaxHealth; // 몬스터 최대 체력
    public int curHealth; // 몬스터 현재 체력
    public Transform target; // 따라갈 타겟
    public bool isChase; // 추적 여부

    Rigidbody rigid; // 리지드 바디
    BoxCollider boxCollider; // 박스 콜라이더
    Material mat; // 재질
    NavMeshAgent nav; // NavAgent가 경로를 그리기 위한 바탕
    Animator anim; // 애니메이터

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 리지드바디 초기화
        boxCollider = GetComponent<BoxCollider>(); // 박스콜라이더 초기화
        mat = GetComponentInChildren<MeshRenderer>().material; // material은 MeshRenderer에서 가져옴
        nav = GetComponent<NavMeshAgent>(); // 네비게이션 초기화
        anim = GetComponentInChildren<Animator>(); // 애니메이터 초기화

        Invoke("ChaseStart", 2); // 2초 뒤에 CahseStart 함수 시작
    }

    void ChaseStart() // 추적 시작 함수
    {
        isChase = true; // 추적 여부 트루
        anim.SetBool("isWalk", true); // 애니메이션 isWalk 값 true로
    }

    void Update()
    {
        if(isChase) // 2초뒤에 추적 시작하게 되면, 목표위치 지정 해주는 함수
        {
            nav.SetDestination(target.position); // 도착할 목표 위치 지정해주는 함수 -> 타겟의 위치로 설정
        }
    }

    void FreezeVelocity() // 속도, 회전력 고정
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero; // 속도 고정
            rigid.angularVelocity = Vector3.zero; // rigidbody 회전력을 0으로 고정
        }
    }

    void FixedUpdate() // 프레임마다 속도, 회전력 고정
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee") // 근접무기일경우
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage; // 근접무기 데미지 만큼 체력 마이너스
            Vector3 reactVec = transform.position - other.transform.position; // 맞은 방향 구하는 코드
            
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet") // 총알일경우
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage; // 총알 데미지 만큼 체력 마이너스
            Vector3 reactVec = transform.position - other.transform.position; // 맞은 방향 구하는 코드
            Destroy(other.gameObject); // 몬스터 맞추면 총알 삭제
            StartCoroutine(OnDamage(reactVec));
        }

        IEnumerator OnDamage(Vector3 reactVec)
        {
            mat.color = Color.red; // 빨간색으로
            yield return new WaitForSeconds(0.1f); // 0.1초 대기

            if(curHealth > 0 ) // 피가 남아있으면
            {
                mat.color = Color.white; // 원래 색으로
            }
            else
            {
                mat.color = Color.gray; // 죽으면 회색으로
                gameObject.layer = 12; // 죽으면 레이어 14번으로 -> EnemyDead
                isChase = false; // 추적 그만
                nav.enabled = false; // 사망 리액션을 위해 navAgent 비활성

                anim.SetTrigger("doDie"); // doDie 애니메이션 실행

                reactVec = reactVec.normalized; // 1로 노멀라이즈드
                reactVec += Vector3.up; // 위쪽으로
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); // 넉백 적용

                Destroy(gameObject, 2); // 2초뒤 사라지게
            }
        }
    }
}
