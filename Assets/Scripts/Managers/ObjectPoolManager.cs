using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public enum PoolKey
{
    EnemyPointer, CommonEnemy, ShieldEnemy, LaserEnemy, DangerousWall, PlayerBullet,
    FireVFX, DestroyVFX, FinalBoss, FinalLaserEnemy
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [Header("Debug")]
    private static ObjectPoolManager _instance;
    
    private Dictionary<PoolKey, IObjectPool<GameObject>> _pools = new();

    [Header("Settings")]
    public List<PoolDetailsSO> poolDetails_SO = new();

    public override void Awake()
    {
        base.Awake();
        UseDetailsRegisterPool();
    }

    private void UseDetailsRegisterPool()
    {
        foreach (var data in poolDetails_SO)
            RegisterPool(data.poolKey, data.prefab, data.defaultCapacity, data.maxSize); 
    }

    // 註冊一個物件池
    private void RegisterPool(PoolKey key, GameObject prefab, int defaultCapacity = 10, int maxSize = 50)
    {
        if (_pools.ContainsKey(key))
        {
            Debug.LogWarning($"Pool with key {key} already exists.");
            return;
        }

        var pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, parent: transform),
            actionOnGet: obj => obj.SetActive(true),
            actionOnRelease: obj => obj.SetActive(false),
            actionOnDestroy: obj => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools.Add(key, pool);
    }

    // 取得物件
    public GameObject GetObject(PoolKey key, Vector3 position = default, Quaternion rotation = default)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"Pool with key {key} does not exist.");
            return null;
        }
        var obj = pool.Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        return obj;
    }

    // 回收物件
    public void ReleaseObject(PoolKey key, GameObject obj)
    {
        if (!_pools.TryGetValue(key, out var pool))
        {
            Debug.LogError($"Pool with key {key} does not exist.");
            return;
        }
        pool.Release(obj);
    }
}