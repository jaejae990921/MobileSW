using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon }; // enum 열거형 타입, { } 안에 데이터를 열거하듯이 작성
    public Type type;
    public int value;

    void Update() {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);    
    }

}
