using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Settings")] 
    public GameObject enemyPrefab;
    public Vector2 minSpawnRange;
    public Vector2 maxSpawnRange;

    private void Awake()
    {
        Invoke("SpawnEnemy", 0.5f);
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, new Vector3(Random.Range(minSpawnRange.x, maxSpawnRange.x),
            Random.Range(minSpawnRange.y, maxSpawnRange.y)), Quaternion.identity);
    }
}
