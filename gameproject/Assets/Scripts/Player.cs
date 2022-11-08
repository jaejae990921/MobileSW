using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;

    float hAxis; //input Axis ���� ���� �������� ����
    float vAxis;
    bool wDown;
    bool jDown;

    bool isjump; //���� �Ѱ輳��



    Vector3 moveVec;

    Rigidbody rigid; //����ȿ���� ��������
    Animator anim; 


    void Awake()
    {
        rigid = GetComponent<Rigidbody>(); //���ʿ� �ֱ� ������ inchildren ���ص���
        anim = GetComponentInChildren<Animator>();
    }


    void Start()
    {
        
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis���� ������ ��ȯ�ϴ� �Լ�(�����ʿ��� ��Ʈ��)
        vAxis = Input.GetAxisRaw("Vertical"); //(���Ʒ� ��Ʈ��)
        wDown = Input.GetButton("Walk"); //shift ���� ���¿����� �ȰԵ�
        jDown = Input.GetButtonDown("Walk"); //������� ����
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //���ÿ� �������� 1�� ��ȯ

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
        if (jDown && !isjump)  // ���� �Ѱ輳��
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); //���� �Ŀ� ����
            isjump = true;
        }
    }

    OnColl

}
