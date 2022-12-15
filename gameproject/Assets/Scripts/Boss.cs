using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook; //방향그대로 유지

    Vector3 lookVec; //플레이어 움직임 예측 벡터 변수 생성
    Vector3 tauntVec;



    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
        
    }


    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            //플레이어 입력값으로 예측 벡터값 생성
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec); //점프 공격할 때 목표지점으로 이동하도록 로직 추가
    }

    IEnumerator Think() //행동 패턴을 결정해주는 코루틴 생성
    {
        yield return new WaitForSeconds(0.1f); //난이도 조정할때 필요함

        int ranAction = Random.Range(0, 5); //행동 패턴을 만들기위해 랜덤함수 호출
        switch (ranAction)
        {
            case 0:
            case 1:
                //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot() //미사일 발사
    {
        anim.SetTrigger("doShot"); //애니메이션 적용
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation); //미사일 생성
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation); //미사일 생성
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think()); //패턴이 끝나면 다음 패턴을 위해 다시 think 코르틴 실행
    }

    IEnumerator RockShot() //돌굴리기
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think()); //패턴이 끝나면 다음 패턴을 위해 다시 think 코르틴 실행
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;  //점프 공격을 할 위치를 변수에 저장

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");  //애니메이션 적용

        yield return new WaitForSeconds(0.6f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think()); //패턴이 끝나면 다음 패턴을 위해 다시 think 코르틴 실행
    }
}
