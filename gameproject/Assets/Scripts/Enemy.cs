using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //enum���� Ÿ���� ������ ������ ����
    public Type enemyType;
    public int maxHealth; //ü�°� �����ͽ��� ���� ���� ����
    public int curHealth;
    public int score;
    public GameManager manager;

    public Transform target; //��ǥ�� �� ����
    public BoxCollider meleeArea; //�ݶ��̴��� ���� ���� �߰�
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public bool isDamage;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs; //��ü ��
    public NavMeshAgent nav;
    public Animator anim; //�ִϸ��̼�

    void Awake() //�ʱ�ȭ
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {

        if (nav.enabled && enemyType != Type.D) //�׺���̼��� Ȱ��ȭ �Ǿ���������
        {
            nav.SetDestination(target.position); //������ ��ǥ ��ġ ���� �Լ�
            nav.isStopped = !isChase; //�Ϻ��ϰ� ���ߵ��� �ۼ�
        }
    }


    void FreezeVelocity() //�̵��� ���� ���� �ʵ����ϴ� ����
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //ȸ���ӵ��� vector3 ���η� �����ϸ� ȸ���ӵ��� 0���� �ϱ� ������ ������ ���� ������ ������.
        }
    }
    void Targetting()
    { 

        if(!isDead && enemyType != Type.D)
        {

            //ShpereCast()�� ������, ���̸� ������ ���� ����
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType) //Ÿ���� ��ġ�� ���ϱ� ��
            {
                case Type.A:
                    targetRadius = 1.5f; //���� ��
                    targetRange = 3f; //���ݹ���
                    break;
                case Type.B:
                    targetRadius = 1f; //���� ��
                    targetRange = 10f; //���ݹ���
                    break;
                case Type.C:
                    targetRadius = 0.5f; //���� ��
                    targetRange = 25f; //���ݹ���
                    break;
            }


            RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, //�ڽ��� ��ġ
                targetRadius,
                transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack && !isDead) //rayHit ������ �����Ͱ� ������ ���� �ڸ�ƾ ����
            {

                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack() //���� ����
    {
       

        isChase = false; //���Ͱ� ������
        isAttack = true; //���Ͱ� ������
        anim.SetBool("isAttack", true); //���� �ִϸ��̼� ����

        switch (enemyType) //Ÿ���� ��ġ�� ���ϱ�
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f); //�ִϸ��̼� �۵��� ���� �����̸� ��
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f); //�ִϸ��̼� �۵��� ���� �����̸� ��
                meleeArea.enabled = false;
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f); //�ִϸ��̼� �۵��� ���� �����̸� ��
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse); //���� ���� 20�Ŀ�
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        

   

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

   
    }

    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity(); //�÷��̾ �ڵ����� ȸ���ϴ°� ���� �Լ�
        
    }

    void OnTriggerEnter(Collider other) //���ƿ��� �Ѿ�, �ظ�
    {
        
        // ���������� collider�� OnTriggerEnter�� 2�� ����Ǵ� ���װ� �־� ����
        // isDamage ������ ���� ù���� �������� ���� ������ ����� ���� �� �ٽ� ����ɼ� �ֵ��� ����
        if (other.tag == "Melee") //�������� ���� ������
        {
            if(isDamage) { return; } // �������� ���� ���̸� ���� �ȵ�
            isDamage = true;        // �������� false�� true�� �ٲ��ְ� ������ ���� ����
            Weapon weapon = other.GetComponent<Weapon>(); //�浹 ����� ��ũ��Ʈ�� ������ damage���� ü�¿� ����
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(meleeOnDamage(reactVec));

        }
        else if (other.tag == "Bullet") //���Ÿ� ���� ������
        {
            

            Bullet bullet = other.GetComponent<Bullet>(); //�浹 ����� ��ũ��Ʈ�� ������ damage���� ü�¿� ����
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //�Ѿ��� ��� ���� ������� ���� �ǵ��� �ڵ�


            StartCoroutine(rangeOnDamage(reactVec));
        }
        
    }

    IEnumerator meleeOnDamage(Vector3 reactVec) //�ǰݷ��� ������ ���� �ڸ�ƾ ����
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //���� ������ �ڵ� ��

        yield return new WaitForSeconds(0.1f); //�ð� ���ϴ� �ڵ�

        if (curHealth > 0 && !isDead)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //�������� �ʾ����� ���� �Ͼ�� ���� ������ �ڵ� ��

        }
        else
        {
            isDead = true;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray; //������ ȸ������ ���� ���� ������ �ڵ� ��

            gameObject.layer = 14; //���̾� �״�� 14�� (���̻� ���� �浹 ���� �ʰ�)
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //���� �״� ���������� �ִϸ��̼ǰ� �÷��� ����
            Player player = target.GetComponent<Player>();
            player.score += score;

            switch (enemyType)
            {

                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;


            }

            reactVec = reactVec.normalized; //�������� ���� vector�� ����
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //�Լ��� �˹� �����ϱ� (�������� ���� ��������)

            Destroy(gameObject, 2f); //�׾����� 2�ʵ� �ı��� 

        }

        yield return new WaitForSeconds(0.4f);
        isDamage= false; 
    }

    IEnumerator rangeOnDamage(Vector3 reactVec) //�ǰݷ��� ������ ���� �ڸ�ƾ ����
    {
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //���� ������ �ڵ� ��

        yield return new WaitForSeconds(0.1f); //�ð� ���ϴ� �ڵ�

        if (curHealth > 0 && !isDead)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //�������� �ʾ����� ���� �Ͼ�� ���� ������ �ڵ� ��

        }
        else
        {
            isDead = true;
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray; //������ ȸ������ ���� ���� ������ �ڵ� ��

            gameObject.layer = 14; //���̾� �״�� 14�� (���̻� ���� �浹 ���� �ʰ�)
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //���� �״� ���������� �ִϸ��̼ǰ� �÷��� ����
            Player player = target.GetComponent<Player>();
            player.score += score;

            switch (enemyType)
            {

                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;


            }

            reactVec = reactVec.normalized; //�������� ���� vector�� ����
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //�Լ��� �˹� �����ϱ� (�������� ���� ��������)

            Destroy(gameObject, 2f); //�׾����� 2�ʵ� �ı��� 

        }

        yield return new WaitForSeconds(0.01f);
        isDamage = false;
    }

}
