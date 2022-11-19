using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public GameObject[] weapons; //플레이어 무기관련 변수
    public bool[] hasWeapons; //플레이어 무기관련 변수
    public GameObject[] grenades; //공전하는 물체를 컨트롤하기 위해 배열변수 생성
    public int hasGrenades; //수류탄

    //아이템 변수 선언
    public int ammo; //탄약
    public int coin; //동전
    public int health; //체력


    //각 수치의 최대값을 저장할 변수 생성
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis; //input Axis 값을 받을 전역변수 선언
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1; //무기교체 1번장비
    bool sDown2; //무기교체 2번장비
    bool sDown3; //무기교체 3번장비

    bool isJump; //점프 한계설정
    bool isDodge; //회피
    bool isSwap; //무기교체동안에 아무것도 못하게함

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid; //물리효과를 내기위해
    Animator anim;

    GameObject nearObject; //트리거 된 아이템을 저장하기 위한 변수 선언
    GameObject equipWeapon; //기존에 장착된 무기를 저장하는 변수를 선언, 활용
    int equipWeaponIndex = -1;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //위쪽에 있기 때문에 inchildren 안해도됨
        anim = GetComponentInChildren<Animator>();
    }


   
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis값을 정수로 반환하는 함수(오른쪽왼쪽 컨트롤)
        vAxis = Input.GetAxisRaw("Vertical"); //(위아래 컨트롤)
        wDown = Input.GetButton("Walk"); //shift 누른 상태에서만 걷게됨
        jDown = Input.GetButtonDown("Jump"); //누른즉시 점프
        iDown = Input.GetButtonDown("Interation"); //e키를 누르면 iDown 활성화됨 (Edit -> project setting)
        sDown1 = Input.GetButtonDown("Swap1"); //무기 1번키를 받음
        sDown2 = Input.GetButtonDown("Swap2"); //무기 2번키를 받음
        sDown3 = Input.GetButtonDown("Swap3"); //무기 3번키를 받음
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //동시에 눌렀을때 1로 변환

        if (isDodge)
            moveVec = dodgeVec; // 회피를 하고 있을경우 움직임 벡터 -> 회피방향 벡터로 바뀌는 코드(회피하면서 다른 방향키 사용불가)
        
        if (isSwap) //무기 교체시 움직임도 멈춤
            moveVec = Vector3.zero;

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)  // 점프 한계설정  (움직임이 zero일때 그냥 점프, 액션 도중에 다른 액션 실행되지 않게 설계)
        {
            rigid.AddForce(Vector3.up * 18, ForceMode.Impulse); //점프 파워 설정
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)  // 점프 한계설정 (움직임이 zero가 아닐때 "회피(Dodge)")
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

    void Swap() //무기 교체 함수
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //0을 중복해서 누르는것
            return; //실행하면안됌
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //1을 중복해서 누르는것
            return; //실행하면안됌
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) //2을 중복해서 누르는것
            return; //실행하면안됌

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0; //1번키 누르면 인덱스 0
        if (sDown2) weaponIndex = 1; //2번키 누르면 인덱스 1
        if (sDown3) weaponIndex = 2; //3번키 누르면 인덱스 2

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge){ //점프나 회피중에는 무기를 못바꾸도록 
            if (equipWeapon != null) //빈손일 경우는 실행 하지 않음
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap"); //애니메이터 셋팅

            isSwap = true; //교체동안에 아무것도 못하게 함

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut() //교체 시간이 끝나고 다시 다른 작업을 할수 있도록 함
    {
        isSwap = false;
    }
    
    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge) //플레이어 근처에 물건이 있다, 점프나 회피때 상호작용 못하게 막음.
        {
            if(nearObject.tag == "Weapon") //무기를 가지고 있다
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true; //무기 인덱스를 트루로 바꿈

                Destroy(nearObject); //아이템 정보를 가져와서 해당 무기 입수를 체크
            }
        }
    }
    
    void OnCollisionEnter(Collision collision) //이벤트 함수로 착지 구현
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false; //바닥에 닿으면 점프를 할 수 있음
        }
    }

    void OnTriggerEnter(Collider other) //트리거 이벤트
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //enum 타입에 맞게 아이템 수치를 플레이어 수치에 적용하기
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value; //enum 타입에 맞게 아이템 수치를 플레이어 수치에 적용하기
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value; //enum 타입에 맞게 아이템 수치를 플레이어 수치에 적용하기
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true); //수류탄 개수대로 공전체가 활성화 되도록 구현
                    hasGrenades += item.value; //enum 타입에 맞게 아이템 수치를 플레이어 수치에 적용하기
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject); //먹은 아이템은 삭제
        }
    }

    void OnTriggerStay(Collider other) //트리거 이벤트
    {
        if (other.tag == "Weapon") //weapon 태그를 조건으로 하여 로직 작성
            nearObject = other.gameObject; //near오브젝트에 저장
        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other) //트리거 이벤트
    {
        if (other.tag == "Weapon") //영역에서 벗어났을때 널값을 줘서 비움
            nearObject = null;
    }
}
