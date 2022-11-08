using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;

    float hAxis; //input Axis 값을 받을 전역변수 선언
    float vAxis;
    bool wDown;
    bool jDown;

    bool isjump; //점프 한계설정



    Vector3 moveVec;

    Rigidbody rigid; //물리효과를 내기위해
    Animator anim; 


    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //위쪽에 있기 때문에 inchildren 안해도됨
        anim = GetComponentInChildren<Animator>();
    }


    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis값을 정수로 반환하는 함수(오른쪽왼쪽 컨트롤)
        vAxis = Input.GetAxisRaw("Vertical"); //(위아래 컨트롤)
        wDown = Input.GetButton("Walk"); //shift 누른 상태에서만 걷게됨
        jDown = Input.GetButtonDown("Walk"); //누른즉시 점프
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //동시에 눌렀을때 1로 변환

        if (wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime; //shift 누르면 속도 느려짐


 

        anim.SetBool("isRun", moveVec != Vector3.zero); //뛰는 상태는 0이 아니면 뜀
        anim.SetBool("isWalk", wDown); //걷는 상태 구현
    }

    void Turn()
    {
        //기본 회전 구현
        transform.LookAt(transform.position + moveVec); //우리가 나아가는 방향으로 바로 바라본다.

    }

    void Jump()
    {
        if (jDown && !isjump)  // 점프 한계설정
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //점프 파워 설정
            isjump = true;
        }
    }

    OnColl

}
