using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // ����, ���Ÿ� ����
    public Type type; // ����Ÿ��
    public int damage; // ������
    public float rate; // ���ݼӵ�
    public BoxCollider meleeArea; // �������� ����
    public TrailRenderer trailEffect; // �������� ȿ��

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); // �ڷ�ƾ ����
            StartCoroutine("Swing"); // �ڷ�ƾ ����
        }
    }

    IEnumerator Swing() // ������ �Լ� Ŭ����
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; // �ڽ��ݶ��̴� Ȱ��ȭ
        trailEffect.enabled = true; // ����Ʈ Ȱ��ȭ

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    // �Ϲ��Լ� : Use() ���η�ƾ -> Swing() �����ƾ -> Use() ���η�ƾ
    // �ڷ�ƾ : Use() ���η�ƾ + Swing() �ڷ�ƾ (���ý���)

    // yield�� ����� �����ϴ� Ű����
    // yield break �� �ڷ�ƾ Ż��
    // yield return null 1������ ���
    // new WaitForSeconds(0.1f) 0.1�� ���
}
