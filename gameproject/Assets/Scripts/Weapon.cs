using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type; //����Ÿ��
    public int damage; //������
    public float rate; //����
    public BoxCollider meleeArea; //����
    public TrailRenderer trailEffect; //ȿ��

   
    // Update is called once per frame
    void Update()
    {
        
    }
}
