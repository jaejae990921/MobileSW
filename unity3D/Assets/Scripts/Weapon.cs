using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // ����, ���Ÿ� ����
    public Type type; // ����Ÿ��
    public int damage; // ������
    public float rate; // ���ݼӵ�
    public int maxAmmo; // �ִ� ź��
    public int curAmmo; // ���� ź��

    public BoxCollider meleeArea; // �������� ����
    public TrailRenderer trailEffect; // �������� ȿ��
    public Transform bulletPos; // �Ѿ� �߻� ��ġ
    public GameObject bullet;// �Ѿ�
    public Transform bulletCasePos; // ź�� ���� ��ġ
    public GameObject bulletCase;// ź��

    public void Use()
    {
        if (type == Type.Melee) // ���������϶�
        {
            StopCoroutine("Swing"); // �ڷ�ƾ ����
            StartCoroutine("Swing"); // �ڷ�ƾ ����
        }
        else if (type == Type.Range && curAmmo > 0) // ����Ÿ�� Range�̸鼭 ź���� 0���� Ŭ ��
        {
            curAmmo--; // ź�� -1
            StartCoroutine("Shot"); // Shot �ڷ�ƾ ����
        }
    }

    IEnumerator Swing() // ������ �Լ� Ŭ����
    {
        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = true; // �ڽ��ݶ��̴� Ȱ��ȭ
        trailEffect.enabled = true; // ����Ʈ Ȱ��ȭ

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // #1.�Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // �Ҹ��������� ��ġ�� ������ �Ҹ��� �ν���Ʈ�� ����
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; // �Ѿ� ������ bulletPos�� ������ ���� �ӵ��� 50
        yield return null; // 1������ ���

        // #2.ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation); // ź���������� ��ġ�� ������ ź�Ǹ� �ν���Ʈ�� ����
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); // �ٱ��������� ������ �� + ��¦ ���������� ��
        caseRigid.AddForce(caseVec, ForceMode.Impulse); // caseRigid�� ��������� ���� ������
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ź�� ȸ��
    }

    // �Ϲ��Լ� : Use() ���η�ƾ -> Swing() �����ƾ -> Use() ���η�ƾ
    // �ڷ�ƾ : Use() ���η�ƾ + Swing() �ڷ�ƾ (���ý���)

    // yield�� ����� �����ϴ� Ű����
    // yield break �� �ڷ�ƾ Ż��
    // yield return null 1������ ���
    // new WaitForSeconds(0.1f) 0.1�� ���
}
