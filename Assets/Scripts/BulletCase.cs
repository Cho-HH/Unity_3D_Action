using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCase : MonoBehaviour, IPoolObject
{
    [SerializeField] private string idName;
    private bool isPool;
    public string IdName
    {
        get { return idName; }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isPool)
        {
            return;
        }

        if (collision.gameObject.tag == "Floor")
        {
            Invoke("Return", 3.0f);
            isPool = true;
        }
    }
    void Return()
    {
        BulletCaseManager.instance.ReturnPool(this);
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
