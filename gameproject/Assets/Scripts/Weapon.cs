using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type; //무기타입
    public int damage = 20; //데미지
    public float rate; // 원거리 무기 공속
    public float meleerate; // 근접무기 공격속도 
    public BoxCollider meleeArea; //범위
    public TrailRenderer trailEffect; //효과
    public Transform bulletPos; // 총알 발사 위치
    public GameObject bullet; // 총알 
    public Transform bulletCasePos; // 탄피 배출 위치 
    public GameObject bulletCase; // 탄피
    public int maxAmmo; //무기 스크립트에 전체 탄약
    public int curAmmo; //현재 탄약 변수 선언
    
    public void Use() //무기사용 로직
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing"); //로직이 꼬이지 않게 중지를 먼저함
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0) //남아있는 탄의 개수가 0보다 클때 쏨
        {
            curAmmo--; //발사했을때 ammo(탄) 소모
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() //근접공격 휘두를때 
    {
        trailEffect.enabled = true;
        //1
        yield return new WaitForSeconds(0.4f); // 앞으로 내려갈때 
        meleeArea.enabled = true; //활성화 컨트롤
        
        //2
        yield return new WaitForSeconds(0.1f); // 휘두르는 속도 조절 ★
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);  // 휘두르는 속도 조절 ★
        trailEffect.enabled = false;

        
    }
    //Use() 메인루틴 -> Swing() 서브루틴 -> use() 메인루틴
    //Use() 메인루틴 + Swing() 코루틴 (Co-Op)

    IEnumerator Shot()
    {
        //#1 총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); //총알 인스턴스화 하기
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50; //인스턴스화된 총알에 속도 적용하기(총알이 날아가는 속도)
        yield return null; //코르틴은 yield가 없으면 에러가 뜸

        //#2. 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3); //인스턴스화된 탄피에 랜덤한 힘 가하기
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //탄피 회전
    }

    public void wpUpgrade() // 망치 공격력 증가 
    {
        damage += 100;
    }

    public  void gunspeed() // 원거리 무기 공격속도 증가
    {
        rate *= 0.7f;
    }

}
