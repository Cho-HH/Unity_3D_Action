using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    public override void Attack()
    {
        base.Attack();
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
