using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // 근접, 원거리 공격
    public Type type; // 무기타입
    public int damage; // 데미지
    public float rate; // 공격속도
    public int maxAmmo; // 최대 탄약
    public int curAmmo; // 현재 탄약

    public BoxCollider meleeArea; // 근접무기 범위
    public TrailRenderer trailEffect; // 근접무기 효과
    public Transform bulletPos; // 총알 발사 위치
    public GameObject bullet;// 총알
    public Transform bulletCasePos; // 탄피 배출 위치
    public GameObject bulletCase;// 탄피

    public void Use()
    {
        if (type == Type.Melee) // 근접무기일때
        {
            StopCoroutine("Swing"); // 코루틴 정지
            StartCoroutine("Swing"); // 코루틴 시작
        }
        else if (type == Type.Range && curAmmo > 0) // 무기타입 Range이면서 탄약이 0보다 클 때
        {
            curAmmo--; // 탄약 -1
            StartCoroutine("Shot"); // Shot 코루틴 시작
        }
    }

    IEnumerator Swing() // 열거형 함수 클래스
    {
        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = true; // 박스콜라이더 활성화
        trailEffect.enabled = true; // 이팩트 활성화

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // #1.총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // 불릿포지션의 위치와 각도에 불릿을 인스턴트로 생성
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; // 총알 방향은 bulletPos의 포워드 방향 속도는 50
        yield return null; // 1프레임 대기

        // #2.탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation); // 탄피포지션의 위치와 각도에 탄피를 인스턴트로 생성
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); // 바깥방향으로 랜덤한 값 + 살짝 위쪽으로의 힘
        caseRigid.AddForce(caseVec, ForceMode.Impulse); // caseRigid에 즉발적으로 힘들 가해줌
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 탄피 회전
    }

    // 일반함수 : Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴
    // 코루틴 : Use() 메인루틴 + Swing() 코루틴 (동시실행)

    // yield는 결과를 전달하는 키워드
    // yield break 로 코루틴 탈출
    // yield return null 1프레임 대기
    // new WaitForSeconds(0.1f) 0.1초 대기
}
