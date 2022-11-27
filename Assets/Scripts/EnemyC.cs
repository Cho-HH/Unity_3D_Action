using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyC : Enemy
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePos;

    private float initSpeed;

    private Transform targetPos;

    protected override void Awake()
    {
        base.Awake();
        initSpeed = nav.speed;
        targetPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void Attack()
    {
        if (isAttacking)
        {
            return;
        }

        base.Attack();
        nav.speed = 0;
        StartCoroutine(AttackRoutine());
    }
    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject missile = Instantiate(bullet, firePos.position, firePos.rotation);

        yield return new WaitForSeconds(1.0f);
        nav.speed = initSpeed;
        isAttack = false;
        isAttacking = false;
        anim.SetBool("isAttack", false);
    }
}
