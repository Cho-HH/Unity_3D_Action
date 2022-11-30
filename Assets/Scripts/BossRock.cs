using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossRock : EnemyBullet
{
    private float angularPower = 2.0f;
    private float scaleValue = 0.1f;
    private bool isShoot;

    public override void Awake()
    {
        base.Awake();
        isShoot = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GainPowerTimer());
    }

    public override void FixedUpdate()
    {
        if (!isShoot)
        {
            angularPower += 0.5f;
            scaleValue += 0.015f;
            transform.localScale = Vector3.one * scaleValue;
            rb.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
        }
    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }
}
