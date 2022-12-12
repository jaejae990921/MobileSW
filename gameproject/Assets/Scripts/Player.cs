using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public FixedJoystick joy; // ���̽�ƽ
    public float speed;
    public GameObject[] weapons; //�÷��̾� ������� ����
    public bool[] hasWeapons; //�÷��̾� ������� ����
    public GameObject[] grenades; //�����ϴ� ��ü�� ��Ʈ���ϱ� ���� �迭���� ����
    public int hasGrenades; //����ź
    public Camera followCamera; //�÷��̾ ����ī�޶� ���� ����
    public GameManager manager;

    //������ ���� ����
    public int ammo; //ź��
    public int health; //ü��
    public int score; //����

    //�� ��ġ�� �ִ밪�� ������ ���� ����
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis; //input Axis ���� ���� �������� ����
    float vAxis;

    bool fDown; //Ű�Է�
    bool rDown; //������ ����
    bool iDown;
    bool sDown1; //���ⱳü 1�����
    bool sDown2; //���ⱳü 2�����
    bool sDown3; //���ⱳü 3�����
    bool jDodge; // ȸ��

    bool isDodge; //ȸ�ǿ���
    bool isSwap; //���ⱳü���ȿ� �ƹ��͵� ���ϰ���
    bool isReload;
    bool isFireReady = true; //�����غ�
    bool isBorder; //�� �浹 �÷��� ����
    bool isDamage; //���� Ÿ��
    public bool isShop;
    bool isDead;
    bool isSwing = false;
    bool isShot;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid; //����ȿ���� ��������
    Animator anim;

    MeshRenderer[] meshs; //�迭 ���� �߰�


    GameObject nearObject; //Ʈ���� �� �������� �����ϱ� ���� ���� ����
    public Weapon equipWeapon; //������ ������ ���⸦ �����ϴ� ������ ����, Ȱ��  

    public GameObject bullet;
    Bullet bulletdamage;

    //int equipWeaponIndex = -1;
    float fireDelay; //���ݵ�����

    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //���ʿ� �ֱ� ������ inchildren ���ص���
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        Defaultbullet();
        // 
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
       // PlayerPrefs.SetInt("MaxScore", 12050);
    }

    void Defaultbullet()
    {
        bulletdamage = bullet.GetComponent<Bullet>();
        bulletdamage.damage = 7;
    }

   
    void Update()
    {
        GetInput();
        Move();
        Turn();
        //Attack();
        //Reload(); //������ �Լ�
        //Dodge();
        //Swap();
        Interation();
    }

    void GetInput()
    {
        hAxis = joy.Horizontal; // Axis���� ������ ��ȯ�ϴ� �Լ�(�����ʿ��� ��Ʈ��)
        vAxis = joy.Vertical; //(���Ʒ� ��Ʈ��)
        //hAxis = Input.GetAxisRaw("Horizontal");
        //vAxis = Input.GetAxisRaw("Vertical");

        //fDown = Input.GetButton("Fire1"); //��� ���� ������ ���� (down�� ���� �ڴ����� �־ �۵� ������)
        rDown = Input.GetButtonDown("Reload"); // r��ư�� ������ ��������.
        iDown = Input.GetButtonDown("Interation"); //eŰ�� ������ iDown Ȱ��ȭ�� (Edit -> project setting)
        sDown1 = Input.GetButtonDown("Swap1"); //���� 1��Ű�� ����
        sDown2 = Input.GetButtonDown("Swap2"); //���� 2��Ű�� ����
        sDown3 = Input.GetButtonDown("Swap3"); //���� 3��Ű�� ����
        jDodge = Input.GetButton("Jump");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //���ÿ� �������� 1�� ��ȯ

        if (isDodge)
            moveVec = dodgeVec; // ȸ�Ǹ� �ϰ� ������� ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� �ڵ�(ȸ���ϸ鼭 �ٸ� ����Ű ���Ұ�)
        
        if (isShot || isSwing || isSwap || isReload || !isFireReady || isShop || isDead) //���� ��ü�� �����ӵ� ���� ,������, �����߿��� �̵��Ұ�
            moveVec = Vector3.zero;

        if (!isBorder) //�÷��� ������ �̵� ���� �������� Ȱ���ϱ�.
        {
            transform.position += moveVec * speed * Time.deltaTime; //shift ������ �ӵ� ������
        }


        anim.SetBool("isRun", moveVec != Vector3.zero); //�ٴ� ���´� 0�� �ƴϸ� ��
    }



    void Turn()
    {
        //#1. Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); //�츮�� ���ư��� �������� �ٷ� �ٶ󺻴�.
    }

    public void Attack()
    {
        if (equipWeapon == null) //�տ� �ƹ��͵� ������
            return;

        else if(equipWeapon.type == Weapon.Type.Melee) // ���������϶�
        {
            //fireDelay += Time.deltaTime;
            //isFireReady = equipWeapon.meleerate < fireDelay; //���ݵ����̿� �ð��� �����ְ� ���ݰ��� ���θ� Ȯ��
            
            
            if (!isSwing && !isDodge && !isSwap && !isShop && !isDead) //�����Ҷ� ���� �������� �� ����
            {
                isSwing = true;
                equipWeapon.Use(); //������ �����Ǹ� ���⿡ �ִ� �Լ� ����
                anim.SetTrigger("doSwing"); //���� Ÿ�Կ� ���� �ٸ� Ʈ���� ����
                //fireDelay = 0; //���� �����̸� 0���� ������ ���� ���ݱ��� ��ٸ����� �ۼ�
                Invoke("SwingOut", 0.8f);
            }
        }
        else // ���Ÿ� �����϶�
        {
            if (equipWeapon.curAmmo == 0) // �Ѿ��� ������ ����
            {
                Reload(); 
            }

            else
            {
                //fireDelay += Time.deltaTime;
                //isFireReady = equipWeapon.rate < fireDelay; //���ݵ����̿� �ð��� �����ְ� ���ݰ��� ���θ� Ȯ��

                if (!isShot && !isDodge && !isSwap && !isShop && !isDead) //�����Ҷ� ���� �������� �� ����
                {
                    isShot = true;
                    equipWeapon.Use(); //������ �����Ǹ� ���⿡ �ִ� �Լ� ����
                    anim.SetTrigger("doShot"); //���� Ÿ�Կ� ���� �ٸ� Ʈ���� ����
                    //fireDelay = 0; //���� �����̸� 0���� ������ ���� ���ݱ��� ��ٸ����� �ۼ�
                    Invoke("ShotOut", equipWeapon.rate);
                }
            }
        }
    }

    void ShotOut()
    {
        isShot = false;
    }

    void SwingOut()
    {
        isSwing = false;
    }

    void Reload() //���� �Լ� ����
    {
        if (equipWeapon == null) //�տ� ���Ⱑ ������ ������ �ȉ�
            return;

        if (equipWeapon.type == Weapon.Type.Melee) //���� ������ ��� ������ �ȉ�
            return;

        //�츮�� �Ѿ��� ���� ������ ���� ammo ������ ����.

        if(!isDodge && !isSwap && isFireReady && !isShop && !isDead) //����, ȸ��, ���� ���涧�� ������ �Ұ�.
        {
            isReload = true;
            anim.SetTrigger("doReload"); //�ִϸ����� Ʈ���� ȣ��� �÷��׺��� ��ȭ �ۼ�
            
            Invoke("ReloadOut", 1.6f); //�����ð� ���� 3�� ��
        }
    }

    void ReloadOut()
    {
        //int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo; //����� ź�� ��
        //ammo -= reAmmo; //�÷��̾ �����ϰ� �ִ� ź�� �������.
        isReload = false;
    }

    public void Dodge()
    {
        if (moveVec != Vector3.zero && !isDodge && !isSwap && !isShop && !isDead)  // ���� �Ѱ輳�� (�������� zero�� �ƴҶ� "ȸ��(Dodge)")
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

    public void Swap() //���� ��ü �Լ�
    {
        //if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) //0�� �ߺ��ؼ� �����°�
        //  return; //�����ϸ�ȉ�
        //if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) //1�� �ߺ��ؼ� �����°�
        //   return; //�����ϸ�ȉ�
        //if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) //2�� �ߺ��ؼ� �����°�
        //   return; //�����ϸ�ȉ�

        //int weaponIndex = -1;
        //if (sDown1) weaponIndex = 0; //1��Ű ������ �ε��� 0
        //if (sDown2) weaponIndex = 1; //2��Ű ������ �ε��� 1
        //if (sDown3) weaponIndex = 2; //3��Ű ������ �ε��� 2
        if (equipWeapon == null) return;

        if (!hasWeapons[0] || !hasWeapons[2]) return;

        if (!isDodge && !isDead)
        {
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }

            if (equipWeapon.type == Weapon.Type.Melee)
            {
                equipWeapon = weapons[2].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap"); //�ִϸ����� ����

                isSwap = true; //��ü���ȿ� �ƹ��͵� ���ϰ� ��

                Invoke("SwapOut", 0.4f);
            }
            else
            {
                equipWeapon = weapons[0].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap"); //�ִϸ����� ����

                isSwap = true; //��ü���ȿ� �ƹ��͵� ���ϰ� ��

                Invoke("SwapOut", 0.4f);
            }
        }
    }

    void SwapOut() //��ü �ð��� ������ �ٽ� �ٸ� �۾��� �Ҽ� �ֵ��� ��
    {
        isSwap = false;
    }
    
    void Interation() // e������ �����ϴ°�
    {
        if(nearObject != null && !isDodge && !isDead) //�÷��̾� ��ó�� ������ �ִ�, ������ ȸ�Ƕ� ��ȣ�ۿ� ���ϰ� ����.
        {
            if(nearObject.tag == "Weapon") //���⸦ ������ �ִ�
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true; //���� �ε����� Ʈ��� �ٲ�

                Destroy(nearObject); //������ ������ �����ͼ� �ش� ���� �Լ��� üũ
            }

            if (!hasWeapons[0] && hasWeapons[2] && equipWeapon == null)
            {
                equipWeapon = weapons[2].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap");

                isSwap = true;

                Invoke("SwapOut", 0.4f);
            }

            if (!hasWeapons[2] && hasWeapons[0] && equipWeapon == null)
            {
                equipWeapon = weapons[0].GetComponent<Weapon>();
                equipWeapon.gameObject.SetActive(true);

                anim.SetTrigger("doSwap");

                isSwap = true;

                Invoke("SwapOut", 0.4f);
            }
        }
        
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; //ȸ���ӵ��� vector3 ���η� �����ϸ� ȸ���ӵ��� 0���� �ϱ� ������ ������ ���� ������ ������.
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green); //������ġ, ��� ����, ray����, ����
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); //�� ��ü�� �浹�� �ϰ� �Ǹ� bool ���� true�� �ȴ�. true���� move���ٰ� ���Ѱ��� ��
    }

    void FixedUpdate()
    {
        FreezeRotation(); //�÷��̾ �ڵ����� ȸ���ϴ°� ���� �Լ�
        StopToWall(); //�� �����ϴ°� ���� �Լ�
    }



    void OnTriggerEnter(Collider other) //Ʈ���� �̺�Ʈ
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value; //enum Ÿ�Կ� �°� ������ ��ġ�� �÷��̾� ��ġ�� �����ϱ�
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
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
        else if (other.tag == "EnemyBullet") //OnTriggerEnter()�� enemybullet ��� �߰�
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage; //bullet ��ũ��Ʈ ��Ȱ���Ͽ� ������ ����

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk)); //�ڸ�ƾ ����.
            }
            if (other.GetComponent<Rigidbody>() != null)  //�̻��� ���ݽ� ������ �����
            {
                Destroy(other.gameObject);
            }
        }
    }

    IEnumerator OnDamage(bool isBossAtk) //���׼��� ���� �ڷ�ƾ ���� �� ȣ��
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshs) //��� ������ ������ ����
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); //�ǰ� �ڷ�ƾ���� �˹��� addforce()�� ����

        if (health <= 0 && !isDead)
        {
            OnDie();
        }

        yield return new WaitForSeconds(1f); //����Ÿ�� ����

        isDamage = false;
        
        foreach (MeshRenderer mesh in meshs) //��� ������ ������ ����
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;

        
    }

    private void OnDie() // �״� ��� 
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other) //Ʈ���� �̺�Ʈ
    {
        if (other.tag == "Weapon" || other.tag == "Shop") //weapon �±׸� �������� �Ͽ� ���� �ۼ�
            nearObject = other.gameObject; //near������Ʈ�� ����
    }

    void OnTriggerExit(Collider other) //Ʈ���� �̺�Ʈ
    {
        if (other.tag == "Weapon") //�������� ������� �ΰ��� �༭ ���
            nearObject = null;
      
    }

    public void setShop(bool state) // ���� �̿� �±�
    {
        isShop = state;
    }

    public void bullUpgrade() // �Ѿ� ������ ����
    {
        bulletdamage.damage += 10;
    }

    public void speedUpgrade() // �̵��ӵ� ����
    {
        speed *= 2;
    }
    public void maxbullUpgrade() // �ִ� źâ ����
    {
        equipWeapon.gameObject.SetActive(false);
        equipWeapon = weapons[2].GetComponent<Weapon>();
        equipWeapon.gameObject.SetActive(true);
        equipWeapon.maxAmmo += 10;
        equipWeapon.curAmmo = equipWeapon.maxAmmo;
    }
    public void maxhpUpgrade() // �ִ�ü�� ����
    {
        maxHealth += 50;
    }


}
