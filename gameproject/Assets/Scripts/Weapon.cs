using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type; //무기타입
    public int damage; //데미지
    public float rate; //공속
    public BoxCollider meleeArea; //범위
    public TrailRenderer trailEffect; //효과

   
    public void Use() //무기사용 로직
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing"); //로직이 꼬이지 않게 중지를 먼저함
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
      
        //1
        yield return new WaitForSeconds(0.1f); //결과를 전달하는 키워드(1프레임대기)
        meleeArea.enabled = true; //활성화 컨트롤
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    //Use() 메인루틴 -> Swing() 서브루틴 -> use() 메인루틴
    //Use() 메인루틴 + Swing() 코루틴 (Co-Op)
}
