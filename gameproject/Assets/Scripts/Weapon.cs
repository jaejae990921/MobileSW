using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type; //����Ÿ��
    public int damage; //������
    public float rate; //����
    public BoxCollider meleeArea; //����
    public TrailRenderer trailEffect; //ȿ��
    public Transform bulletPos; //�Ѿ�, ź�� ���õ� ���� ����
    public GameObject bullet; //�Ѿ�, ź�� ���õ� ���� ����
    public Transform bulletCasePos; //�Ѿ�, ź�� ���õ� ���� ����
    public GameObject bulletCase; //�Ѿ�, ź�� ���õ� ���� ����
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
      
        //1
        yield return new WaitForSeconds(0.1f); //����� �����ϴ� Ű����(1�����Ӵ��)
        meleeArea.enabled = true; //Ȱ��ȭ ��Ʈ��
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f); // �ֵθ��� �ӵ� ���� ��
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
        bulletRigid.velocity = bulletPos.forward * 50; //�ν��Ͻ�ȭ�� �Ѿ˿� �ӵ� �����ϱ�(�Ѿ� �߻� �ӵ� ��)
        yield return null; //�ڸ�ƾ�� yield�� ������ ������ ��

        //#2. ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //�ν��Ͻ�ȭ�� ź�ǿ� ������ �� ���ϱ�
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //ź�� ȸ��
    }
}
