using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; //������ Ÿ��
    public Type type;
    public int value;


    Rigidbody rigid; //������ ���� �浹�� ����ϴ� �ݶ��̴��� �浹�Ͽ� ���� �߻� fix
    SphereCollider sphereCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision) //����ȿ�� ����
    {
        if(collision.gameObject.tag == "Floor") //�ٴڿ� ���� ����
        {
            rigid.isKinematic = true; //rigidbody�� �ܺ� ���� ȿ���� ���� �������� ���ϰ���.
            sphereCollider.enabled = false;
        }
    }
}
