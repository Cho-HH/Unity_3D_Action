using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBullet : EnemyBullet
{
    private Transform target;
    private NavMeshAgent nav;

    public override void Awake()
    {
        base.Awake();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        StartCoroutine(DestroyMissile());
    }

    // Update is called once per frame
    void Update()
    {
        nav.SetDestination(target.position);        
    }

    IEnumerator DestroyMissile()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
