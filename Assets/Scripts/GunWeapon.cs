using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EGUN_TYPE
{
    HANDGUN,
    MACHINEGUN
}

public class GunWeapon : Weapon
{   
    [SerializeField] private EGUN_TYPE type;

    [SerializeField] Transform bulletPos;
    [SerializeField] Transform bulletCasePos;

    [SerializeField] int reloadAmmo;
    [SerializeField] int curAmmo;

    [SerializeField] int poolNum;

    public EGUN_TYPE Type
    {
        get { return type; }
    }
    public int ReloadAmmo
    {
        get { return reloadAmmo; }
    }
    public int CurAmmo
    {
        get { return curAmmo; }
        set { curAmmo = value; }
    }
    public override bool CanAttack()
    {
        if (curAmmo == 0)
        {
            return false;
        }

        curAmmo--;
        anim.SetTrigger("doShot");
        GameObject bullet = BulletManager.instance.Spawn(poolNum, bulletPos.position, bulletPos.rotation).gameObject;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(bulletPos.forward * 100.0f,ForceMode.Impulse);
        TrailRenderer trailRenderer = bullet.GetComponent<TrailRenderer>();
        trailRenderer.Clear();

        GameObject bulletCase = BulletCaseManager.instance.Spawn(0, bulletCasePos.position, bulletCasePos.rotation).gameObject;
        rb = bulletCase.GetComponent<Rigidbody>();
        float rand = Random.Range(2.0f, 4.0f);
        rb.AddForce(bulletCasePos.right * rand + Vector3.up * 2.0f, ForceMode.Impulse);
        rb.AddTorque(transform.forward * 100.0f, ForceMode.Impulse);

        return true;
    }
}
