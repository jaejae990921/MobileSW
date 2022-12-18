using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //enum으로 타입을 나누고 변수를 생성
    public Type enemyType;
    public int maxHealth; //체력과 컴포넌스를 담을 변수 선언
    public int curHealth;
    public int score;
    public GameManager manager;

    public Transform target; //목표가 될 변수
    public BoxCollider meleeArea; //콜라이더를 담을 변수 추가
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public bool isDamage;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs; //물체 색
    public NavMeshAgent nav;
    public Animator anim; //애니메이션

    void Awake() //초기화
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void ChaseStart() // 추적 시작 함수
    {
        isChase = true; // 추석 여부
        anim.SetBool("isWalk", true); // 애니메이션 iswalk 
    }

    void Update()
    {

        if (nav.enabled && enemyType != Type.D) //네비게이션이 활성화 되어있을때만
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
    void Targetting()  // Player를 목표로 움직이는 기능
    { 

        if(!isDead && enemyType != Type.D)
        {

            //ShpereCast()의 반지름, 길이를 조정할 변수 선언
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType) //타게팅 수치를 정하기 ★
            {
                case Type.A:
                    targetRadius = 1.5f; //공격 폭
                    targetRange = 3f; //공격범위
                    break;
                case Type.B:
                    targetRadius = 1f; //공격 폭
                    targetRange = 10f; //공격범위
                    break;
                case Type.C:
                    targetRadius = 0.5f; //공격 폭
                    targetRange = 25f; //공격범위
                    break;
            }


            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, //자신의 위치
                targetRadius,
                transform.forward, targetRange, LayerMask.GetMask("Player")); // 타겟은 Player

            if (rayHits.Length > 0 && !isAttack && !isDead) //rayHit 변수에 데이터가 들어오면 공격 코르틴 실행
            {

                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack() //몬스터 공격
    {
       

        isChase = false; //몬스터가 정지함
        isAttack = true; //몬스터가 공격함
        anim.SetBool("isAttack", true); //공격 애니메이션 적용

        switch (enemyType) //타게팅 수치를 정하기
        {
            case Type.A: // 일반 공격 몬스터
                yield return new WaitForSeconds(0.2f); //애니메이션 작동을 위한 딜레이를 줌
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f); //애니메이션 작동을 위한 딜레이를 줌
                meleeArea.enabled = false;
                break;

            case Type.B: // 돌진 공격 몬스터
                yield return new WaitForSeconds(0.1f); //애니메이션 작동을 위한 딜레이를 줌
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); //돌격 구현 20파워
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(2f);
                break;

            case Type.C: // 원거리 공격 몬스터 (미사일)
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        

   

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
        
        // 근접무기의 collider의 OnTriggerEnter이 2번 실행되는 버그가 있어 수정
        // isDamage 변수를 통해 첫번 공격이후 근접 무기의 모션이 끝난 뒤 다시 실행될수 있도록 수정
        if (other.tag == "Melee") //근접무기 공격 맞을때
        {
            if(isDamage) { return; } // 데미지가 들어가는 중이면 실행 안됨
            isDamage = true;        // 데미지가 false면 true로 바꿔주고 나머지 공격 실행
            Weapon weapon = other.GetComponent<Weapon>(); //충돌 상대의 스크립트를 가져와 damage값을 체력에 적용
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(meleeOnDamage(reactVec));

        }
        else if (other.tag == "Bullet") //원거리 공격 맞을때
        {
            

            Bullet bullet = other.GetComponent<Bullet>(); //충돌 상대의 스크립트를 가져와 damage값을 체력에 적용
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //총알의 경우 적과 닿았을때 삭제 되도록 코드


            StartCoroutine(rangeOnDamage(reactVec));
        }
        
    }

    IEnumerator meleeOnDamage(Vector3 reactVec) // 근접 무기 피격 로직
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //색깔 입히는 코드 ★

        yield return new WaitForSeconds(0.1f); //시간 정하는 코드

        if (curHealth > 0 && !isDead) // 죽지 않았을 때
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //아직죽지 않았을시 색상 하얀색 색깔 입히는 코드 ★

        }
        else // 죽었을 
        {
            isDead = true;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray; //죽으면 회색으로 변경 색깔 입히는 코드 ★

            gameObject.layer = 14; //레이어 그대로 14번 (더이상 물리 충돌 하지 않고)
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //적이 죽는 시점에서도 애니메이션과 플래그 셋팅
            Player player = target.GetComponent<Player>();
            player.score += score;

            switch (enemyType)
            {

                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;


            }

            reactVec = reactVec.normalized; //리엑션을 위해 vector을 선언
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //함수로 넉백 구현하기 (뒷쪽으로 힘이 가해진다)

            Destroy(gameObject, 2f); //죽었을시 2초뒤 파괴됨 

        }

        yield return new WaitForSeconds(0.4f);
        isDamage= false; 
    }

    IEnumerator rangeOnDamage(Vector3 reactVec) // 원거리 무기 피격에 대한 로직 
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //색깔 입히는 코드 ★

        yield return new WaitForSeconds(0.1f); //시간 정하는 코드

        if (curHealth > 0 && !isDead) // 몬스터가 죽지 않았을 때
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //아직죽지 않았을시 색상 하얀색 색깔 입히는 코드 ★

        }
        else // 몬스터가 죽었을 
        {
            isDead = true;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray; //죽으면 회색으로 변경 색깔 입히는 코드 ★

            gameObject.layer = 14; //레이어 그대로 14번 (더이상 물리 충돌 하지 않고)
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //적이 죽는 시점에서도 애니메이션과 플래그 셋팅
            Player player = target.GetComponent<Player>();
            player.score += score;

            switch (enemyType)
            {

                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;


            }

            reactVec = reactVec.normalized; //리엑션을 위해 vector을 선언
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //함수로 넉백 구현하기 (뒷쪽으로 힘이 가해진다)

            Destroy(gameObject, 2f); //죽었을시 2초뒤 파괴됨 

        }

        yield return new WaitForSeconds(0.01f);
        isDamage = false;
    }

}
