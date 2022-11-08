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

    bool isJump; //���� �Ѱ輳��
    bool isDodge; //ȸ��


    Vector3 moveVec;
    Vector3 dodgeVec;

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
        Dodge();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // Axis���� ������ ��ȯ�ϴ� �Լ�(�����ʿ��� ��Ʈ��)
        vAxis = Input.GetAxisRaw("Vertical"); //(���Ʒ� ��Ʈ��)
        wDown = Input.GetButton("Walk"); //shift ���� ���¿����� �ȰԵ�
        jDown = Input.GetButtonDown("Jump"); //������� ����
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; //���ÿ� �������� 1�� ��ȯ

        if (isDodge)
            moveVec = dodgeVec; // ȸ�Ǹ� �ϰ� ������� ������ ���� -> ȸ�ǹ��� ���ͷ� �ٲ�� �ڵ�(ȸ���ϸ鼭 �ٸ� ����Ű ���Ұ�)

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
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)  // ���� �Ѱ輳��  (�������� zero�϶� �׳� ����, �׼� ���߿� �ٸ� �׼� ������� �ʰ� ����)
        {
            rigid.AddForce(Vector3.up * 18, ForceMode.Impulse); //���� �Ŀ� ����
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge)  // ���� �Ѱ輳�� (�������� zero�� �ƴҶ� "ȸ��(Dodge)")
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

    void OnCollisionEnter(Collision collision) //�̺�Ʈ �Լ��� ���� ����
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false; //�ٴڿ� ������ ������ �� �� ����
        }
    }

}
