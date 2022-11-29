using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FixedJoystick joy; // 조이스틱
    public float speed; // 속도

    public GameObject[] weapons; // 무기 배열
    public bool[] hasWeapons; // 무기를 가졌는지 bool 배열

    float hAxis; // x축값
    float vAxis; // y축값

    Vector3 moveVec; // 이동 벡터
    Vector3 dodgeVec; // 회피 벡터

    Animator anim; // 애니메이션

    Rigidbody rigid; // 물리효과
    bool jDodge; // 회피
    bool isDodge; // 회피 여부

    GameObject nearObject; // 트리거된 아이템을 저장

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // 물리효과
        anim = GetComponentInChildren<Animator>(); // 애니메이션
    }

    // Update is called once per frame
    void Update()
    {
        GetInput(); // 입력
        Move(); // 이동
        Turn(); // 회전
        Dodge(); // 회피
        Interaction();
    }

    void GetInput() { // 입력 
        //hAxis = joy.Horizontal; // x축 // 조이스틱
        //vAxis = joy.Vertical; // z축 // 조이스틱
        jDodge = Input.GetButtonDown("Jump"); // 점프 입력받기

        hAxis = Input.GetAxisRaw("Horizontal"); // x축 키보드
        vAxis = Input.GetAxisRaw("Vertical"); // y축 키보드
    }

    void Move() { // 이동구현
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // x y z 축, 모든방향 속도 똑같게 하기위해 normalized

        if (isDodge) { // 회피중이라면
            moveVec = dodgeVec; // 이동방향을 회피방향으로 고정
        }
        transform.position += moveVec * speed * Time.deltaTime; // 이동속도
        
        anim.SetBool("isRun", moveVec != Vector3.zero); // 가만히 있는 상태가 아니면 isRun이 되게 함
    }

    void Turn() { // 회전구현
        transform.LookAt(transform.position + moveVec); // lookat : 지정된 vector 값으로 회전시켜주는 함수
    }

    void Dodge() { // 회피구현
    if (jDodge && moveVec != Vector3.zero && !isDodge ) { // 스페이스바 눌렀을때, 제자리가 아닐때, 회피중이 아닐때
        dodgeVec = moveVec; // 회피하는 방향을 현재 바라보는 방향으로 고정
        speed = speed * 2; // 이동속도 2배 
        anim.SetTrigger("doDodge"); // 회피 애니메이션 실행
        isDodge = true; // 회피중 true
        Invoke("DodgeOut", 0.5f); // 시간차 함수호출, "호출될 함수", 기다릴시간
        // 0.5초마다 쓸 수 있음
        }
    }

    void DodgeOut() { // 회피동작 끝
        speed = speed * 0.5f; // 이동속도 원상복귀
        isDodge = false; // 회피중 false로 변경
    }

    void Interaction() {
        if (nearObject != null && !isDodge) // 가까이있는 오브젝트가 null이 아닐때, 회피중이 아닐때
        {
            if (nearObject.tag == "Weapon") // 태그가 무기일때
            {
                Item item = nearObject.GetComponent<Item>(); // nearObject의 컴포넌트 가져옴, Item은 아이템 전용 
                int weaponeIndex = item.value; // 오브젝트의 value값을 인덱스로 저장
                hasWeapons[weaponeIndex] = true; // 해당 무기를 가졌으니 bool값을 true로 변경

                Destroy(nearObject); // 먹은 무기를 사라지게함
            }
        }
    }

    void OnTriggerStay(Collider other) { // 트리거 이벤트
        if(other.tag == "Weapon") { // 닿아있는 오브젝트의 tag가 무기면
            nearObject = other.gameObject; // 가까이있는 오브젝트로 저장
        }
    }

    void OnTriggerExit(Collider other) { // 영역을 벗어났을 때
        if(other.tag == "Weapon") { // 닿아있는 오브젝트의 tag가 무기면
            nearObject = null; // 가까이있는 오브젝트를 null로
        }
    }

    void sex() {
        
    }
}