using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    public PoolKey poolKey;

    public virtual void ReturnToPool()
    {
        Debug.Log("Back " + gameObject.name);
        ObjectPoolManager.Instance.ReleaseObject(poolKey, gameObject);
    }
}