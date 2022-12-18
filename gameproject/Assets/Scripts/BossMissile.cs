using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public Transform target;
    NavMeshAgent nav;
    void Awake()
    {
        nav = GetComponent<NavMeshAgent>(); // 유니티 AI의 navmeshagent를 이용하여 자동 추적
    }


    void Update()
    {
        nav.SetDestination(target.position); // 타겟은 Player
    }
}
