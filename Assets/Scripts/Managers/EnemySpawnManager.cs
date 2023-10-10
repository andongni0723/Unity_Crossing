using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class SpawnPosition
{
    public string name;
    public Vector2 minSpawnRange;
    public Vector2 maxSpawnRange; 
}

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Settings")] 
    public GameObject enemyPrefab;
    public float waitNextSpawnTime = 1.5f;
    public float waitNextWaveTime = 3;
    
    [Space(15)]
    public List<SpawnPosition> SpawnPositionsList = new List<SpawnPosition>();

    private void Awake()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            int spawnCount = Random.Range(1, 5);
            int spawnPositionIndex = Random.Range(0, SpawnPositionsList.Count);
            SpawnPosition currentSpawnPosition = SpawnPositionsList[spawnPositionIndex];

            for (int i = 0; i < spawnCount; i++)
            {
                Instantiate(enemyPrefab, new Vector3(
                        Random.Range(currentSpawnPosition.minSpawnRange.x, currentSpawnPosition.maxSpawnRange.x),
                        Random.Range(currentSpawnPosition.minSpawnRange.y, currentSpawnPosition.maxSpawnRange.y)),
                    Quaternion.identity);
                
                yield return new WaitForSeconds(waitNextSpawnTime);
            }
            
            yield return new WaitForSeconds(waitNextWaveTime);
        }
    }
}
