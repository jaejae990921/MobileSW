using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FixedJoystick joy; // 조이스틱
    public float speed;
    public GameObject[] weapons; //플레이어 무기관련 변수
    public bool[] hasWeapons; //플레이어 무기관련 변수
    public GameObject[] grenades; //공전하는 물체를 컨트롤하기 위해 배열변수 생성
    public int hasGrenades; //수류탄
    public Camera followCamera; //플레이어에 메인카메라 변수 만듬
    public GameManager manager;

    //아이템 변수 선언
    public int ammo; //탄약
    public int health; //체력
    public int score; //점수

    //각 수치의 최대값을 저장할 변수 생성
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis; //input Axis 값을 받을 전역변수 선언
    float vAxis;

    bool fDown; //키입력
    bool rDown; //재장전 변수
    bool iDown;
    bool sDown1; //무기교체 1번장비
    bool sDown2; //무기교체 2번장비
    bool sDown3; //무기교체 3번장비
    bool jDodge; // 회피

    bool isDodge; //회피여부
    bool isSwap; //무기교체동안에 아무것도 못하게함
    bool isReload;
    bool isFireReady = true; //공격준비
    bool isBorder; //벽 충돌 플레그 변수
    bool isDamage; //무적 타임
    public bool isShop;
    bool isDead;
    bool isSwing = false;
    bool isShot;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid; //물리효과를 내기위해
    Animator anim;

    MeshRenderer[] meshs; //배열 변수 추가


    GameObject nearObject; //트리거 된 아이템을 저장하기 위한 변수 선언
    public Weapon equipWeapon; //기존에 장착된 무기를 저장하는 변수를 선언, 활용  

    public GameObject bullet;
    Bullet bulletdamage;

    //int equipWeaponIndex = -1;
    float fireDelay; //공격딜레이

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //위쪽에 있기 때문에 inchildren 안해도됨
        anim = GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트 가져옴
        meshs = GetComponentsInChildren<MeshRenderer>();
        Defaultbullet();
        //PlayerPrefs.SetInt("MaxScore", 100); // 임의로 맥스 스코어 설정
    }

    void Defaultbullet()// 총알데미지 초기화 함수
    {
        bulletdamage = bullet.GetComponent<Bullet>(); // Bullet 스크립트 가져오기
        bulletdamage.damage = 7; // 총알데미지의 damage를 7로 설정
    }

   
    void Update()
    {
        GetInput(); //입력
        Move(); // 이동
        Turn(); // 회전
        //Attack();
        //Reload(); //재장전 함수
        //Dodge();
        //Swap();
        Interation(); // 무기 흭득
    }

    void GetInput()
    {
        hAxis = joy.Horizontal; // 조이스틱 x축 입력
        vAxis = joy.Vertical; // 조이스틱 y축 입력
        //hAxis = Input.GetAxisRaw("Horizontal");  // x축 키보드입력, Axis값을 정수로 반환하는 함수
        //vAxis = Input.GetAxisRaw("Vertical"); // y축 키보드입력

        //fDown = Input.GetButton("Fire1"); //마운스 왼쪽 누르면 어택 (down을 빼면 꾹누르고 있어도 작동 가능함)
        rDown = Input.GetButtonDown("Reload"); // r버튼을 누르면 재장전함.
        iDown = Input.GetButtonDown("Interation"); //e키를 누르면 iDown 활성화됨 (Edit -> project setting)
        sDown1 = Input.GetButtonDown("Swap1"); //무기 1번키를 받음
        sDown2 = Input.GetButtonDown("Swap2"); //무기 2번키를 받음
        sDown3 = Input.GetButtonDown("Swap3"); //무기 3번키를 받음
        jDodge = Input.GetButton("Jump"); //스페이스바 점프키
    }

    void Move() // 이동
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //동시에 눌렀을때 1로 변환

        if (isDodge)
            moveVec = dodgeVec; // 움직임 벡터 -> 회피방향 벡터로 바뀌는 코드(회피하면서 다른 방향키 사용불가)
        
        if (isShot || isSwing || isSwap || isReload || !isFireReady || isShop || isDead) // 원거리공격, 근접공격, 무기교체, 재장전, 상점이용, 죽었을때 이동불가
            moveVec = Vector3.zero;

        if (!isBorder) //플래그 변수를 이동 제한 조건으로 활용하기.
        {
            transform.position += moveVec * speed * Time.deltaTime;  // 이동속도
        }


        anim.SetBool("isRun", moveVec != Vector3.zero); // 이동벡터가 0이 아닐경우(가만히있는경우제외) isRun 애니메이션 동작
    }



    void Turn() // 캐릭터 회전
    {
        //#1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // 지정된 벡터값으로 회전시켜주는 함수사용
    }

    public void Attack() // 공격
    {
        if (equipWeapon == null) //손에 무기가 아무것도 없으면 리턴
            return;

        else if(equipWeapon.type == Weapon.Type.Melee) // 근접무기일때
        {
            //fireDelay += Time.deltaTime;
            //isFireReady = equipWeapon.meleerate < fireDelay; //공격딜레이에 시간을 더해주고 공격가능 여부를 확인
            
            
            if (!isSwing && !isDodge && !isSwap && !isShop && !isDead)  // 스윙, 회피, 무기교체, 상점이용, 죽음상태가 아닐때 공격 불가
            {
                isSwing = true;
                equipWeapon.Use(); // weapon에 있는 Use 함수 실행
                anim.SetTrigger("doSwing");// doSwing 애니메이션 동작
                //fireDelay = 0; //공격 딜레이를 0으로 돌려서 다음 공격까지 기다리도록 작성
                Invoke("SwingOut", 0.8f); // swingout 함수를 0.8초뒤에 실행
            }
        }
        else // 원거리 무기일때
        {
            if (equipWeapon.curAmmo == 0) // 총알이 없으면 장전
            {
                Reload();   // 장정함수 실행
            }

            else
            {
                //fireDelay += Time.deltaTime;
                //isFireReady = equipWeapon.rate < fireDelay; //공격딜레이에 시간을 더해주고 공격가능 여부를 확인

                if (!isShot && !isDodge && !isSwap && !isShop && !isDead)// 공격, 회피, 무기교체, 상점이용, 죽음상태가 아닐때 공격 불가
                {
                    isShot = true;
                    equipWeapon.Use(); // weapon에 있는 Use 함수 실행
                    anim.SetTrigger("doShot");  // doShot 애니메이션 동작
                    //fireDelay = 0; //공격 딜레이를 0으로 돌려서 다음 공격까지 기다리도록 작성
                    Invoke("ShotOut", equipWeapon.rate); // Shotout 함수를 공격속도만큼 뒤에 실행 -> 공격속도가 빠르면 더 일찍 shot 상태를 탈출
                }
            }
        }
    }

    void ShotOut() // Shot 여부를 false로
    {
        isShot = false;
    }

    void SwingOut() // Swing 여부를 false로
    {
        isSwing = false;
    }

    void Reload() //장전 함수 구현
    {
        if (equipWeapon == null) //손에 무기가 없으면 장전 불가
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //근접 무기일 경우 장전 불가
            return;

        //우리는 총알이 없기 때문에 따로 ammo 설정을 안함.

        if(!isDodge && !isSwap && isFireReady && !isShop && !isDead) //점프, 회피, 무기 변경때는 장전이 불가.
        {
            isReload = true;
            anim.SetTrigger("doReload"); // doReload 애니메이션 시작
            
            Invoke("ReloadOut", 1.6f); // 1.6초 뒤에 ReloadOut 함수 실행
        }
    }

    void ReloadOut() // 장전 끝 함수
    {
        //int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo; // 무기의 현재 탄약을 최대 탄약으로 채워줌
        //ammo -= reAmmo; //플레이어가 소지하고 있는 탄은 사라진다
        isReload = false;
    }

    public void Dodge() // 회피 동작
    {
        if (moveVec != Vector3.zero && !isDodge && !isSwap && !isShop && !isDead)  // 점프 한계설정 (움직임이 zero가 아닐때 "회피(Dodge)")
        {
            dodgeVec = moveVec; //무브벡터를 닷지벡터에 대입, 회피중 방향을 못바꾸게함
            speed *= 2;  // 이동 속도 두배
            anim.SetTrigger("doDodge");  // doDodge 함수 실행
            isDodge = true;

            Invoke("DodgeOut", 0.5f);  // 닷지할때 0.4초의 시간차(딜레이)를 줌 <바로 닷지 안돼게 방지 코드>
        }
    }
    
    void DodgeOut() //시간차를 줘서 isDodge를 false
    {
        speed *= 0.5f;
        isDodge = false;
    }

    public void Swap() //무기 교체 함수
    {
        //if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //0을 중복해서 누르는것
        //  return; //실행하면안
        //if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //1을 중복해서 누르는것
        //   return; //실행하면안
        //if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) //2을 중복해서 누르는것
        //   return; //실행하면안

        //int weaponIndex = -1;
        //if (sDown1) weaponIndex = 0; //1번키 누르면 인덱스 0
        //if (sDown2) weaponIndex = 1; //2번키 누르면 인덱스 1
        //if (sDown3) weaponIndex = 2; //3번키 누르면 인덱스 2
        if (equipWeapon == null) return; 

        if (!hasWeapons[0] || !hasWeapons[2]) return;  // 해머와 머신건을 갖고있지 않으면 리턴

        if (!isDodge && !isDead) // 회피중, 죽음상태가 아닐때
        {
            if (equipWeapon != null) // 손에 무기를 들고있으면
            {
                equipWeapon.gameObject.SetActive(false); // 손에 든 무기를 안보이게 설정
            }

            if (equipWeapon.type == Weapon.Type.Melee)
            {
                equipWeapon = weapons[2].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap"); // 무기교체 애니메이션 실행

                isSwap = true; // 교체 여부를 true로

                Invoke("SwapOut", 0.4f); // 교체 끝 함수를 0.4초 뒤에 호출
            }
            else
            {
                equipWeapon = weapons[0].GetComponent<Weapon>(); // 해머의 Weapone 스크립트를 가져옴
                equipWeapon.gameObject.SetActive(true); // 해머를 보이게 설정

                anim.SetTrigger("doSwap");  // 무기교체 애니메이션 실행

                isSwap = true; //교체동안에 아무것도 못하게 함

                Invoke("SwapOut", 0.4f); // 교체 끝 함수를 0.4초 뒤에 호출
            }
        }
    }

    void SwapOut() // 무기교체 끝 함수
    {
        isSwap = false;
    }
    
    void Interation()  // 상호작용 함수, 무기에 다가가면 자동습득 및 자동으로 무기로 
    {
        if(nearObject != null && !isDodge && !isDead)  // 근처에 오브젝트 존재, 회피중X, 죽음상태X 이면
        {
            if(nearObject.tag == "Weapon") // 오브젝트의 태그가 Weapon이면
            {
                Item item = nearObject.GetComponent<Item>(); // 해당 오브젝트의 Item 스크립트 가져옴
                int weaponIndex = item.value;  // 해당 무기의 벨류를 웨폰인덱스로 가져옴
                hasWeapons[weaponIndex] = true; // 해당 무기를 가졌다고 true로 변경

                Destroy(nearObject);  // 습득한 아이템을 씬에서 삭제
            }

            if (!hasWeapons[0] && hasWeapons[2] && equipWeapon == null) // 해머습득X, 손에무기X, 서브머신건을 습득했을때
            {
                equipWeapon = weapons[2].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap");  // 무기교체 애니메이션 실행

                isSwap = true;  // 무기교체 여부를 true로
 
                Invoke("SwapOut", 0.4f);
            }

            if (!hasWeapons[2] && hasWeapons[0] && equipWeapon == null) // 서브머신건습득X, 손에무기X, 해머입수시
            {
                equipWeapon = weapons[0].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap");

                isSwap = true;

                Invoke("SwapOut", 0.4f);
            }
        }
        
    }

    void FreezeRotation() // 회전버그 방지 함수
    {
        rigid.angularVelocity = Vector3.zero; //회전속도를 vector3 제로로 설정하면 회전속도를 0으로 하기 때문에 스스로 도는 현상이 없어짐.
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green); //시작위치, 쏘는 방향, ray길이, 색깔
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); //벽 물체랑 충돌을 하게 되면 bool 값이 true가 된다. true값이 move에다가 제한값을 줌
    }

    void FixedUpdate() // 매 프레임마다 호출하는 
    {
        FreezeRotation(); //플레이어가 자동으로 회전하는거 막는 함수
        StopToWall(); //벽 관통하는거 막는 함수
    }



    void OnTriggerEnter(Collider other) //트리거 이벤트
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //enum 타입에 맞게 아이템 수치를 플레이어 수치에 적용하기
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
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
        else if (other.tag == "EnemyBullet") //OnTriggerEnter()에 enemybullet 경우 추가
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage; //bullet 스크립트 재활용하여 데미지 적용

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk)); //코르틴 적용.
            }
            if (other.GetComponent<Rigidbody>() != null)  //미사일 공격시 맞으면 사라짐
            {
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool isBossAtk) // 데미지 맞았을 때
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshs) //모든 재질의 색상을 변경
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); //피격 코루틴에서 넉백을 addforce()로 구현

        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f); //무적타임 조정

        isDamage = false;
        
        foreach (MeshRenderer mesh in meshs) //모든 재질의 색상을 변경
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;

        
    }

    private void OnDie() // 죽는 모셩 
    {
        anim.SetTrigger("doDie"); // 죽는 애니메이션 실행
        isDead = true; // 죽음 상태 설정
        manager.GameOver(); // 게임오버 함수 시작
    }

    void OnTriggerStay(Collider other) //트리거 이벤트
    {
        if (other.tag == "Weapon" || other.tag == "Shop") //weapon 태그를 조건으로 하여 로직 작성
            nearObject = other.gameObject; //near오브젝트에 저장
    }

    void OnTriggerExit(Collider other) //트리거 이벤트
    {
        if (other.tag == "Weapon") //영역에서 벗어났을때 널값을 줘서 비움
            nearObject = null;
      
    }

    public void setShop(bool state) // 상점 이용 태그
    {
        isShop = state;
    }

    public void bullUpgrade() // 총알 데미지 증가
    {
        bulletdamage.damage += 10;
    }

    public void speedUpgrade() // 이동속도 증가
    {
        speed *= 1.2f;
    }
    public void maxbullUpgrade() // 최대 탄창 증가
    {
        equipWeapon.gameObject.SetActive(false);
        equipWeapon = weapons[2].GetComponent<Weapon>();
        equipWeapon.gameObject.SetActive(true);
        equipWeapon.maxAmmo += 10;
        equipWeapon.curAmmo = equipWeapon.maxAmmo;
    }
    public void maxhpUpgrade() // 최대체력 증가
    {
        maxHealth += 50;
        health = maxHealth;
    }


}
