using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : Enemy
{
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
        yield return new WaitForSeconds(1.3f);
        isAttack = false;
        isAttacking = false;
        anim.SetBool("isAttack", false);
    }

    public override void decreaseCnt()
    {
        GameManager.Instance.EnemyACnt--;
    }
}
