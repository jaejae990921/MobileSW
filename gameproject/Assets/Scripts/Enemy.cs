using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public int maxHealth; //체력과 컴포넌스를 담을 변수 선언
    public int curHealth;
    public Transform target; //목표가 될 변수
    public BoxCollider meleeArea; //콜라이더를 담을 변수 추가
    public bool isChase;
    public bool isAttack;



    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat; //물체 색
    NavMeshAgent nav;
    Animator anim; //애니메이션

    void Awake() //초기화
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {

        if (nav.enabled) //네비게이션이 활성화 되어있을때만
        {
            nav.SetDestination(target.position); //도착할 목표 위치 지정 함수
            nav.isStopped = !isChase; //완벽하게 멈추도록 작성
        }
    }


    void FreezeVelocity() //이동을 방해 하지 않도록하는 로직
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //회전속도를 vector3 제로로 설정하면 회전속도를 0으로 하기 때문에 스스로 도는 현상이 없어짐.
        }
    }
    void Targetting()
    {   //ShpereCast()의 반지름, 길이를 조정할 변수 선언
        float targetRadius = 1.5f;
        float targetRange = 3f;

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, //자신의 위치
            targetRadius,
            transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack) //rayHit 변수에 데이터가 들어오면 공격 코르틴 실행
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack() //몬스터 공격
    {
        isChase = false; //몬스터가 정지함
        isAttack = true; //몬스터가 공격함
        anim.SetBool("isAttack", true); //공격 애니메이션 적용

        yield return new WaitForSeconds(0.2f); //애니메이션 작동을 위한 딜레이를 줌
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1f); //애니메이션 작동을 위한 딜레이를 줌
        meleeArea.enabled = false;

   

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

   
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity(); //플레이어가 자동으로 회전하는거 막는 함수
        
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
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //적이 죽는 시점에서도 애니메이션과 플래그 셋팅



            reactVec = reactVec.normalized; //리엑션을 위해 vector을 선언
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //함수로 넉백 구현하기 (뒷쪽으로 힘이 가해진다)

            Destroy(gameObject, 2f); //죽었을시 2초뒤 파괴됨 
        }
    }

}
