using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    //리지드 바디 변수와 회전 파워 , 크기 숫자값 변수 생성 
    float angularPower = 2;
    float scaleValue = 0.1f;

    bool isShoot; //기를 모으고 쏘는 타이밍을 관리할 bool변수 

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f); //2.2초가 되면 쏴라!!
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot) //쏘는 상황이 아닌경우
        {
            angularPower += 0.02f;
            scaleValue += 0.001f; // 바위 크기 변화 해상도에 따라 크기가 달라짐
            transform.localScale = Vector3.one * scaleValue; //while문에서 증가된 값을 트랜스폼, 리지드 바디에 적용
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null; //while문에 꼭 적어야 하는 코드
        }
    }
}
