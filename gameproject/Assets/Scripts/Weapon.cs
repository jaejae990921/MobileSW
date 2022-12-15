using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type; //����Ÿ��
    public int damage = 20; //������
    public float rate; // ���Ÿ� ���� ����
    public float meleerate; // �������� ���ݼӵ� 
    public BoxCollider meleeArea; //����
    public TrailRenderer trailEffect; //ȿ��
    public Transform bulletPos; // �Ѿ� �߻� ��ġ
    public GameObject bullet; // �Ѿ� 
    public Transform bulletCasePos; // ź�� ���� ��ġ 
    public GameObject bulletCase; // ź��
    public int maxAmmo; //���� ��ũ��Ʈ�� ��ü ź��
    public int curAmmo; //���� ź�� ���� ����
    
    public void Use() //������ ����
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing"); //������ ������ �ʰ� ������ ������
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0) //�����ִ� ź�� ������ 0���� Ŭ�� ��
        {
            curAmmo--; //�߻������� ammo(ź) �Ҹ�
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //�������� �ֵθ��� 
    {
        trailEffect.enabled = true;
        //1
        yield return new WaitForSeconds(0.4f); // ������ �������� 
        meleeArea.enabled = true; //Ȱ��ȭ ��Ʈ��
        
        //2
        yield return new WaitForSeconds(0.1f); // �ֵθ��� �ӵ� ���� ��
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);  // �ֵθ��� �ӵ� ���� ��
        trailEffect.enabled = false;

        
    }
    //Use() ���η�ƾ -> Swing() �����ƾ -> use() ���η�ƾ
    //Use() ���η�ƾ + Swing() �ڷ�ƾ (Co-Op)

    IEnumerator Shot()
    {
        //#1 �Ѿ� �߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //�Ѿ� �ν��Ͻ�ȭ �ϱ�
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //�ν��Ͻ�ȭ�� �Ѿ˿� �ӵ� �����ϱ�(�Ѿ��� ���ư��� �ӵ�)
        yield return null; //�ڸ�ƾ�� yield�� ������ ������ ��

        //#2. ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //�ν��Ͻ�ȭ�� ź�ǿ� ������ �� ���ϱ�
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //ź�� ȸ��
    }

    public void wpUpgrade() // ��ġ ���ݷ� ���� 
    {
        damage += 100;
    }

    public  void gunspeed() // ���Ÿ� ���� ���ݼӵ� ����
    {
        rate *= 0.7f;
    }

}
