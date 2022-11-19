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

   
    // Update is called once per frame
    void Update()
    {
        
    }
}
