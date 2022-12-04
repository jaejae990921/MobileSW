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
        nav = GetComponent<NavMeshAgent>(); //초기
    }


    void Update()
    {
        nav.SetDestination(target.position); //추적
    }
}
