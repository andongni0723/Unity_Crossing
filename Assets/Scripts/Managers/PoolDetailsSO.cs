using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolDetailsSO", menuName = "ScriptableObject/PoolDetailsSO", order = 1)]
public class PoolDetailsSO : ScriptableObject
{
    public PoolKey poolKey;
    public GameObject prefab;
    public int defaultCapacity = 10;
    public int maxSize = 50;
}
