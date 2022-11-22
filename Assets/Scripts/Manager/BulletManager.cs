using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class BulletManager : MonoBehaviour
{
    public static BulletManager instance;

    private PoolManager poolManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        poolManager = GetComponent<PoolManager>();
    }

    public Bullet Spawn(int poolsNum, Vector3 spawnPos, Quaternion spawnRot)
    {
        Bullet bullet = poolManager.GetFromPool<Bullet>(poolsNum);
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = spawnRot;
        return bullet;
    }

    public void ReturnPool(Bullet clone)
    {               
        poolManager.TakeToPool<Bullet>(clone.IdName, clone);
    }
}
