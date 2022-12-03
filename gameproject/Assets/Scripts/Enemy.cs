using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public int maxHealth; //ü�°� �����ͽ��� ���� ���� ����
    public int curHealth;
    public Transform target; //��ǥ�� �� ����
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat; //��ü ��
    NavMeshAgent nav;
    Animator anim; //�ִϸ��̼�

    void Awake() //�ʱ�ȭ
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
       
        if (isChase)
            nav.SetDestination(target.position); //������ ��ǥ ��ġ ���� �Լ�
    }


    void FreezeVelocity() //�̵��� ���� ���� �ʵ����ϴ� ����
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero; //ȸ���ӵ��� vector3 ���η� �����ϸ� ȸ���ӵ��� 0���� �ϱ� ������ ������ ���� ������ ������.
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity(); //�÷��̾ �ڵ����� ȸ���ϴ°� ���� �Լ�
        
    }

    void OnTriggerEnter(Collider other) //���ƿ��� �Ѿ�, �ظ�
    {
        if(other.tag == "Melee") //�������� ���� ������
        {
            Weapon weapon = other.GetComponent<Weapon>(); //�浹 ����� ��ũ��Ʈ�� ������ damage���� ü�¿� ����
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet") //���Ÿ� ���� ������
        {
            Bullet bullet = other.GetComponent<Bullet>(); //�浹 ����� ��ũ��Ʈ�� ������ damage���� ü�¿� ����
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject); //�Ѿ��� ��� ���� ������� ���� �ǵ��� �ڵ�


            StartCoroutine(OnDamage(reactVec));
        }
        
    }

    IEnumerator OnDamage(Vector3 reactVec) //�ǰݷ��� (������ ���� �ڸ�ƾ ����
    {
        mat.color = Color.red; //���� ������ �ڵ� ��
        yield return new WaitForSeconds(0.1f); //�ð� ���ϴ� �ڵ�

        if(curHealth > 0)
        {
            mat.color = Color.white; //�������� �ʾ����� ���� �Ͼ��
        }
        else
        {
            mat.color = Color.gray; //������ ȸ������ ����
            gameObject.layer = 14; //���̾� �״�� 14�� (���̻� ���� �浹 ���� �ʰ�)
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //���� �״� ���������� �ִϸ��̼ǰ� �÷��� ����



            reactVec = reactVec.normalized; //�������� ���� vector�� ����
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse); //�Լ��� �˹� �����ϱ� (�������� ���� ��������)

            Destroy(gameObject, 2f); //�׾����� 2�ʵ� �ı��� 
        }
    }

}
