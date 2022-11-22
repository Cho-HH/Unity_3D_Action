using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class BulletCaseManager : MonoBehaviour
{
    public static BulletCaseManager instance;

    private PoolManager poolManager;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        poolManager = GetComponent<PoolManager>();
    }
    public BulletCase Spawn(int poolsNum, Vector3 spawnPos, Quaternion spawnRot)
    {
        BulletCase bulletCase = poolManager.GetFromPool<BulletCase>(poolsNum);
        bulletCase.transform.position = spawnPos;
        bulletCase.transform.rotation = spawnRot;
        return bulletCase;
    }

    public void ReturnPool(BulletCase clone)
    {
        poolManager.TakeToPool<BulletCase>(clone.IdName, clone);
    }
}
