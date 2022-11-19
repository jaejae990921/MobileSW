using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public GameObject[] weapons; //�÷��̾� ������� ����
    public bool[] hasWeapons; //�÷��̾� ������� ����
    public GameObject[] grenades; //�����ϴ� ��ü�� ��Ʈ���ϱ� ���� �迭���� ����
    public int hasGrenades; //����ź

    //������ ���� ����
    public int ammo; //ź��
    public int coin; //����
    public int health; //ü��


    //�� ��ġ�� �ִ밪�� ������ ���� ����
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis; //input Axis ���� ���� �������� ����
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool sDown1; //���ⱳü 1�����
    bool sDown2; //���ⱳü 2�����
    bool sDown3; //���ⱳü 3�����

    bool isJump; //���� �Ѱ輳��
    bool isDodge; //ȸ��
    bool isSwap; //���ⱳü���ȿ� �ƹ��͵� ���ϰ���

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid; //����ȿ���� ��������
    Animator anim;

    GameObject nearObject; //Ʈ���� �� �������� �����ϱ� ���� ���� ����
    GameObject equipWeapon; //������ ������ ���⸦ �����ϴ� ������ ����, Ȱ��
    int equipWeaponIndex = -1;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //���ʿ� �ֱ� ������ inchildren ���ص���
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
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis���� ������ ��ȯ�ϴ� �Լ�(�����ʿ��� ��Ʈ��)
        vAxis = Input.GetAxisRaw("Vertical"); //(���Ʒ� ��Ʈ��)
        wDown = Input.GetButton("Walk"); //shift ���� ���¿����� �ȰԵ�
        jDown = Input.GetButtonDown("Jump"); //������� ����
        iDown = Input.GetButtonDown("Interation"); //eŰ�� ������ iDown Ȱ��ȭ�� (Edit -> project setting)
        sDown1 = Input.GetButtonDown("Swap1"); //���� 1��Ű�� ����
        sDown2 = Input.GetButtonDown("Swap2"); //���� 2��Ű�� ����
        sDown3 = Input.GetButtonDown("Swap3"); //���� 3��Ű�� ����
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //���ÿ� �������� 1�� ��ȯ

        if (isDodge)
            moveVec = dodgeVec; // ȸ�Ǹ� �ϰ� ������� ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� �ڵ�(ȸ���ϸ鼭 �ٸ� ����Ű ���Ұ�)
        
        if (isSwap) //���� ��ü�� �����ӵ� ����
            moveVec = Vector3.zero;

        if (wDown)
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        else
            transform.position += moveVec * speed * Time.deltaTime; //shift ������ �ӵ� ������


        anim.SetBool("isRun", moveVec != Vector3.zero); //�ٴ� ���´� 0�� �ƴϸ� ��
        anim.SetBool("isWalk", wDown); //�ȴ� ���� ����
    }

    void Turn()
    {
        //�⺻ ȸ�� ����
        transform.LookAt(transform.position + moveVec); //�츮�� ���ư��� �������� �ٷ� �ٶ󺻴�.

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)  // ���� �Ѱ輳��  (�������� zero�϶� �׳� ����, �׼� ���߿� �ٸ� �׼� ������� �ʰ� ����)
        {
            rigid.AddForce(Vector3.up * 18, ForceMode.Impulse); //���� �Ŀ� ����
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)  // ���� �Ѱ輳�� (�������� zero�� �ƴҶ� "ȸ��(Dodge)")
        {
            dodgeVec = moveVec; //���꺤�͸� �������Ϳ� ����
            speed *= 2; //���ǵ� �ӵ� �ι�
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f); // �����Ҷ� 0.4���� �ð���(������)�� �� <�ٷ� ���� �ȵŰ� ���� �ڵ�>

        }
    }
    
    void DodgeOut() //�ð����� �༭ isDodge�� false
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap() //���� ��ü �Լ�
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //0�� �ߺ��ؼ� �����°�
            return; //�����ϸ�ȉ�
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //1�� �ߺ��ؼ� �����°�
            return; //�����ϸ�ȉ�
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) //2�� �ߺ��ؼ� �����°�
            return; //�����ϸ�ȉ�

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0; //1��Ű ������ �ε��� 0
        if (sDown2) weaponIndex = 1; //2��Ű ������ �ε��� 1
        if (sDown3) weaponIndex = 2; //3��Ű ������ �ε��� 2

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge){ //������ ȸ���߿��� ���⸦ ���ٲٵ��� 
            if (equipWeapon != null) //����� ���� ���� ���� ����
                equipWeapon.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);

            anim.SetTrigger("doSwap"); //�ִϸ����� ����

            isSwap = true; //��ü���ȿ� �ƹ��͵� ���ϰ� ��

            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut() //��ü �ð��� ������ �ٽ� �ٸ� �۾��� �Ҽ� �ֵ��� ��
    {
        isSwap = false;
    }
    
    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge) //�÷��̾� ��ó�� ������ �ִ�, ������ ȸ�Ƕ� ��ȣ�ۿ� ���ϰ� ����.
        {
            if(nearObject.tag == "Weapon") //���⸦ ������ �ִ�
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true; //���� �ε����� Ʈ��� �ٲ�

                Destroy(nearObject); //������ ������ �����ͼ� �ش� ���� �Լ��� üũ
            }
        }
    }
    
    void OnCollisionEnter(Collision collision) //�̺�Ʈ �Լ��� ���� ����
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false; //�ٴڿ� ������ ������ �� �� ����
        }
    }

    void OnTriggerEnter(Collider other) //Ʈ���� �̺�Ʈ
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //enum Ÿ�Կ� �°� ������ ��ġ�� �÷��̾� ��ġ�� �����ϱ�
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value; //enum Ÿ�Կ� �°� ������ ��ġ�� �÷��̾� ��ġ�� �����ϱ�
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value; //enum Ÿ�Կ� �°� ������ ��ġ�� �÷��̾� ��ġ�� �����ϱ�
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true); //����ź ������� ����ü�� Ȱ��ȭ �ǵ��� ����
                    hasGrenades += item.value; //enum Ÿ�Կ� �°� ������ ��ġ�� �÷��̾� ��ġ�� �����ϱ�
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject); //���� �������� ����
        }
    }

    void OnTriggerStay(Collider other) //Ʈ���� �̺�Ʈ
    {
        if (other.tag == "Weapon") //weapon �±׸� �������� �Ͽ� ���� �ۼ�
            nearObject = other.gameObject; //near������Ʈ�� ����
        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other) //Ʈ���� �̺�Ʈ
    {
        if (other.tag == "Weapon") //�������� ������� �ΰ��� �༭ ���
            nearObject = null;
    }
}
