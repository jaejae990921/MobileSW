using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FixedJoystick joy; // 조이스틱
    public float speed; // 속도

    float hAxis; // x축값
    float vAxis; // y축값
    Vector3 moveVec; // 벡터

    Animator anim; // 애니메이션

    Rigidbody rigid; // 물리효과
    bool jDown; // 점프
    bool isJump; // 점프의 여부

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 물리효과
        anim = GetComponentInChildren<Animator>(); // 애니메이션
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput() { // 입력 
        hAxis = joy.Horizontal; // x축 // 조이스틱
        vAxis = joy.Vertical; // z축 // 조이스틱
        jDown = Input.GetButtonDown("Jump"); // 점프 입력받기
    }

    void Move() { // 이동구현
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // x y z 축

        transform.position += moveVec * speed * Time.deltaTime; // 원래 속도
        
        anim.SetBool("isRun", moveVec != Vector3.zero); // 가만히 있는 상태가 아니면 isRun이 되게 함
    }

    void Turn() { // 회전구현
        transform.LookAt(transform.position + moveVec); // lookat : 지정된 vector 값으로 회전시켜주는 함수
    }

    void Jump() { // 점프구현
        if (jDown && !isJump) { // 점프중이 아닐때만 작동되게
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // AddForce 함수로 물리적인 힘 가하기
            isJump = true;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Floor") { // 바닥에 있으면 isJump를 false로
            isJump = false;
        }
    }
}