using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
   
    void Update()
    {
        transform.Rotate(Vector3.right * 30 * Time.deltaTime); // 미사일이 일자로 발사
    }
}
