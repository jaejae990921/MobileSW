using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Nav ���� Ŭ���� ���

public class Enemy : MonoBehaviour
{
    public int MaxHealth; // ���� �ִ� ü��
    public int curHealth; // ���� ���� ü��
    public Transform target; // ���� Ÿ��
    public bool isChase; // ���� ����

    Rigidbody rigid; // ������ �ٵ�
    BoxCollider boxCollider; // �ڽ� �ݶ��̴�
    Material mat; // ����
    NavMeshAgent nav; // NavAgent�� ��θ� �׸��� ���� ����
    Animator anim; // �ִϸ�����

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>(); // ������ٵ� �ʱ�ȭ
        boxCollider = GetComponent<BoxCollider>(); // �ڽ��ݶ��̴� �ʱ�ȭ
        mat = GetComponentInChildren<MeshRenderer>().material; // material�� MeshRenderer���� ������
        nav = GetComponent<NavMeshAgent>(); // �׺���̼� �ʱ�ȭ
        anim = GetComponentInChildren<Animator>(); // �ִϸ����� �ʱ�ȭ

        Invoke("ChaseStart", 2); // 2�� �ڿ� CahseStart �Լ� ����
    }

    void ChaseStart() // ���� ���� �Լ�
    {
        isChase = true; // ���� ���� Ʈ��
        anim.SetBool("isWalk", true); // �ִϸ��̼� isWalk �� true��
    }

    void Update()
    {
        if(isChase) // 2�ʵڿ� ���� �����ϰ� �Ǹ�, ��ǥ��ġ ���� ���ִ� �Լ�
        {
            nav.SetDestination(target.position); // ������ ��ǥ ��ġ �������ִ� �Լ� -> Ÿ���� ��ġ�� ����
        }
    }

    void FreezeVelocity() // �ӵ�, ȸ���� ����
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero; // �ӵ� ����
            rigid.angularVelocity = Vector3.zero; // rigidbody ȸ������ 0���� ����
        }
    }

    void FixedUpdate() // �����Ӹ��� �ӵ�, ȸ���� ����
    {
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee") // ���������ϰ��
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage; // �������� ������ ��ŭ ü�� ���̳ʽ�
            Vector3 reactVec = transform.position - other.transform.position; // ���� ���� ���ϴ� �ڵ�
            
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet") // �Ѿ��ϰ��
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage; // �Ѿ� ������ ��ŭ ü�� ���̳ʽ�
            Vector3 reactVec = transform.position - other.transform.position; // ���� ���� ���ϴ� �ڵ�
            Destroy(other.gameObject); // ���� ���߸� �Ѿ� ����
            StartCoroutine(OnDamage(reactVec));
        }

        IEnumerator OnDamage(Vector3 reactVec)
        {
            mat.color = Color.red; // ����������
            yield return new WaitForSeconds(0.1f); // 0.1�� ���

            if(curHealth > 0 ) // �ǰ� ����������
            {
                mat.color = Color.white; // ���� ������
            }
            else
            {
                mat.color = Color.gray; // ������ ȸ������
                gameObject.layer = 12; // ������ ���̾� 14������ -> EnemyDead
                isChase = false; // ���� �׸�
                nav.enabled = false; // ��� ���׼��� ���� navAgent ��Ȱ��

                anim.SetTrigger("doDie"); // doDie �ִϸ��̼� ����

                reactVec = reactVec.normalized; // 1�� ��ֶ������
                reactVec += Vector3.up; // ��������
                rigid.AddForce(reactVec * 5, ForceMode.Impulse); // �˹� ����

                Destroy(gameObject, 2); // 2�ʵ� �������
            }
        }
    }
}
