using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") // ÅºÇÇ ¶¥¿¡ ´êÀ¸¸é
        {
            Destroy(gameObject, 0.5f); // 0.5ÃÊ µÚ¿¡ »ç¶óÁü
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall") // ÃÑ¾Ë º®¿¡ ´êÀ¸¸é
        {
            Destroy(gameObject); // ¹Ù·Î »èÁ¦
        }
    }
}