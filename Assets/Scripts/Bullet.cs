using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Bullet : MonoBehaviour, IPoolObject
{
    [SerializeField] private string idName;
    [SerializeField] private int damage;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;

    private bool isPool;
    public string IdName
    {
        get { return idName; }
    }

    public int Damage
    {
        get { return damage; }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPool)
        {
            return;
        }

        if (other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall" || other.gameObject.tag == "Enemy" || other.tag == "Boss")
        {
            rb.velocity = Vector3.zero;
            BulletManager.instance.ReturnPool(this);
            isPool = true;
        }
    }

    public void OnCreatedInPool()
    {
        isPool = false;
    }

    public void OnGettingFromPool()
    {
        isPool = false;
    }
}
