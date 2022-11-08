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

    bool isJump; //점프 한계설정
    bool isDodge; //회피


    Vector3 moveVec;
    Vector3 dodgeVec;

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
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis값을 정수로 반환하는 함수(오른쪽왼쪽 컨트롤)
        vAxis = Input.GetAxisRaw("Vertical"); //(위아래 컨트롤)
        wDown = Input.GetButton("Walk"); //shift 누른 상태에서만 걷게됨
        jDown = Input.GetButtonDown("Jump"); //누른즉시 점프
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //동시에 눌렀을때 1로 변환

        if (isDodge)
            moveVec = dodgeVec; // 회피를 하고 있을경우 움직임 벡터 -> 회피방향 벡터로 바뀌는 코드(회피하면서 다른 방향키 사용불가)

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)  // 점프 한계설정  (움직임이 zero일때 그냥 점프, 액션 도중에 다른 액션 실행되지 않게 설계)
        {
            rigid.AddForce(Vector3.up * 18, ForceMode.Impulse); //점프 파워 설정
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)  // 점프 한계설정 (움직임이 zero가 아닐때 "회피(Dodge)")
        {
            dodgeVec = moveVec; //무브벡터를 닷지벡터에 대입
            speed *= 2; //스피드 속도 두배
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // 닷지할때 0.4초의 시간차(딜레이)를 줌 <바로 닷지 안돼게 방지 코드>

        }
    }
    
    void DodgeOut() //시간차를 줘서 isDodge를 false
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision) //이벤트 함수로 착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false; //바닥에 닿으면 점프를 할 수 있음
        }
    }

}
