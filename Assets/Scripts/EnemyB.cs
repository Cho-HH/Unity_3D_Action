using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyB : Enemy
{
    private float initSpeed;
    private float initAngularSpeed;
    public override void Attack()
    {
        if (isAttacking)
        {
            return;
        }
        base.Attack();
        StartCoroutine(AttackRoutine());        
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        rb.AddForce(transform.forward * 3.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(1.0f);
        isAttack = false;
        isAttacking = false;
        anim.SetBool("isAttack", false);        
    }
}
