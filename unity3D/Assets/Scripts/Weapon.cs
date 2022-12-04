using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // 근접, 원거리 공격
    public Type type; // 무기타입
    public int damage; // 데미지
    public float rate; // 공격속도
    public BoxCollider meleeArea; // 근접무기 범위
    public TrailRenderer trailEffect; // 근접무기 효과

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing"); // 코루틴 정지
            StartCoroutine("Swing"); // 코루틴 시작
        }
    }

    IEnumerator Swing() // 열거형 함수 클래스
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true; // 박스콜라이더 활성화
        trailEffect.enabled = true; // 이팩트 활성화

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    // 일반함수 : Use() 메인루틴 -> Swing() 서브루틴 -> Use() 메인루틴
    // 코루틴 : Use() 메인루틴 + Swing() 코루틴 (동시실행)

    // yield는 결과를 전달하는 키워드
    // yield break 로 코루틴 탈출
    // yield return null 1프레임 대기
    // new WaitForSeconds(0.1f) 0.1초 대기
}
