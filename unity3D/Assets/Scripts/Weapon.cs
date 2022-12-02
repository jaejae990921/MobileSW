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
            Swing();
        }
    }

    IEnumerator Swing() // ������ �Լ� Ŭ����
    {
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    // yield�� ����� �����ϴ� Ű����
    // yield break �� �ڷ�ƾ Ż��
    // yield return null 1������ ���
    // new WaitForSeconds(0.1f) 0.1�� ���
    // �Ϲ��Լ� : Use() ���η�ƾ -> Swing() �����ƾ -> Use() ���η�ƾ
    // �ڷ�ƾ : Use() ���η�ƾ + Swing() �ڷ�ƾ (���ý���)
}
