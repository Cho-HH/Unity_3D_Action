using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected int damage;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private TrailRenderer trailEffect;

    public int Damage
    {
        get { return damage; }
    }

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
       
        yield return new WaitForSeconds(0.4f);
        audio.Play();
        trailEffect.enabled = false;

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;
    }
}
