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

   
    public void Use() //������ ����
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing"); //������ ������ �ʰ� ������ ������
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
      
        //1
        yield return new WaitForSeconds(0.1f); //����� �����ϴ� Ű����(1�����Ӵ��)
        meleeArea.enabled = true; //Ȱ��ȭ ��Ʈ��
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    //Use() ���η�ƾ -> Swing() �����ƾ -> use() ���η�ƾ
    //Use() ���η�ƾ + Swing() �ڷ�ƾ (Co-Op)
}
