using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour // 라운드 진행 테스트를 위한 스크립트
{

    public GameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            manager.StageEnd();
        }
    }
}
