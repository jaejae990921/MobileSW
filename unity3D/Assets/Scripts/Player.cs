using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FixedJoystick joy; // 조이스틱
    public float speed; // 속도

    public GameObject[] weapons; // 무기 배열
    public bool[] hasWeapons; // 무기를 가졌는지 bool 배열

    public int ammo; // 총알
    public int maxAmmo; // 최대총알
    public int health; // 체력
    public int maxHealth; // 최대체력

    float hAxis; // x축값
    float vAxis; // y축값

    Vector3 moveVec; // 이동 벡터
    Vector3 dodgeVec; // 회피 벡터

    Animator anim; // 애니메이션
    Rigidbody rigid; // 물리효과

    bool jDodge; // 회피
    bool isDodge; // 회피 여부
    bool isSwap; // 스왑 여부
    bool isFireReady = true; // 공격 준비

    bool sDown1; // 1번 버튼
    bool sDown2; // 2번 버튼
    bool fDown; // 공격 입력

    GameObject nearObject; // 트리거된 아이템을 저장
    Weapon equipWeapon; // 손에 들고있는 무기 저장
    int equipWeaponIndex = -1;// 손에 들고있는 무기의 인덱스
    float fireDelay; // 공격 딜레이

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
        Attack();
        Dodge(); // 회피
        Swap(); // 무기교체
        Interaction(); // 무기획득
    }

    void GetInput() { // 입력 
        //hAxis = joy.Horizontal; // x축 // 조이스틱
        //vAxis = joy.Vertical; // z축 // 조이스틱
        jDodge = Input.GetButtonDown("Jump"); // 점프 입력받기

        hAxis = Input.GetAxisRaw("Horizontal"); // x축 키보드
        vAxis = Input.GetAxisRaw("Vertical"); // y축 키보드
        
        fDown = Input.GetButtonDown("Fire1");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
    }

    void Move() { // 이동구현
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // x y z 축, 모든방향 속도 똑같게 하기위해 normalized

        if (isDodge) { // 회피중이라면
            moveVec = dodgeVec; // 이동방향을 회피방향으로 고정
        }

        if (isSwap || !isFireReady) // 무기 스왑이거나 공격중이면 가만히
        {
            moveVec = Vector3.zero;
        }


        transform.position += moveVec * speed * Time.deltaTime; // 이동속도
        
        anim.SetBool("isRun", moveVec != Vector3.zero); // 가만히 있는 상태가 아니면 isRun이 되게 함
    }

    void Turn() { // 회전구현
        transform.LookAt(transform.position + moveVec); // lookat : 지정된 vector 값으로 회전시켜주는 함수
    }

    void Attack()
    {
        if (equipWeapon == null) return; // 손에 든 무기가 없으면 리턴

        fireDelay += Time.deltaTime; // 공격딜레이에 시간을 더해주고 공격가능 여부 확인
        isFireReady = equipWeapon.rate < fireDelay; 

        if(fDown && isFireReady && !isDodge && !isSwap) // 공격버튼 눌렀을때, 공격가능할때, 회피나 스왑중이 아닐때
        {
            equipWeapon.Use(); // 무기 Use() 함수 사용
            anim.SetTrigger("doSwing");
            fireDelay = 0; // 공격 딜레이를 0으로 돌려서 다음 공격까지 기다리도록 작성
        }
    }

    void Dodge() { // 회피구현
    if (jDodge && moveVec != Vector3.zero && !isDodge && !isSwap ) { // 스페이스바 눌렀을때, 제자리가 아닐때, 회피중이 아닐때, 스왑중이 아닐때
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

    void Swap() // 무기 교체
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;// 무기가 없거나 같은무기를 들고있는 경우
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;// 무기가 없거나 같은무기를 들고있는 경우

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0; // 1 누르면 웨폰인덱스 0
        if (sDown2) weaponIndex = 1; // 2 누르면 웨폰인덱스 1

        if ((sDown1 || sDown2) && !isDodge && !isSwap) // 1,2번 눌렀을때, 회피중이 아닐때
        {
            if(equipWeapon != null) // 손에 뭘 들고있는 상태라면
            {
                equipWeapon.gameObject.SetActive(false); // 원래 들고있던 무기를 안보이게 설정
            }
            equipWeaponIndex = weaponIndex; // 손에들고있는 무기 인덱스를 입력받은 무기의 인덱스로
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); // 버튼 누른 무기를 손에 들고있는 무기로 설정
            equipWeapon.gameObject.SetActive(true); // 손에 들고 있는 무기를 보이게 설정

            anim.SetTrigger("doSwap");

            isSwap = true; // 스왑여부를 true로

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    { // 스왑동작 끝
        isSwap = false; // 스왑여부 false
    }

    void Interaction() { // 무기획득
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
}