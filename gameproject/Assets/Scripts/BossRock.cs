using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    //������ �ٵ� ������ ȸ�� �Ŀ� , ũ�� ���ڰ� ���� ���� 
    float angularPower = 2;
    float scaleValue = 0.1f;

    bool isShoot; //�⸦ ������ ��� Ÿ�̹��� ������ bool���� 

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f); //2.2�ʰ� �Ǹ� ����!!
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot) //��� ��Ȳ�� �ƴѰ��
        {
            angularPower += 0.02f;
            scaleValue += 0.005f;
            transform.localScale = Vector3.one * scaleValue; //while������ ������ ���� Ʈ������, ������ �ٵ� ����
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null; //while���� �� ����� �ϴ� �ڵ�
        }
    }
}
