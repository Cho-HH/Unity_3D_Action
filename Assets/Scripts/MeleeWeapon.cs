using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected int damage;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private TrailRenderer trailEffect;    

    public override bool CanAttack()
    {
        anim.SetTrigger("doSwing");
        StopCoroutine("Swing");
        StartCoroutine("Swing");
        return true;
    }  

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
}
