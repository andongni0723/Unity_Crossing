using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum SpawnPositionType
{
    Up, Down, Left, Right
}

public enum EnemyKind
{
    Common, Laser, Shield, Wall
}

[Serializable]
public class SpawnPosition
{
    public string name; // Inspector Window
    public SpawnPositionType type;
    public Vector2 minSpawnRange;
    public Vector2 maxSpawnRange; 
    public Quaternion spawnRotation;
}

[Serializable]
public class EnemySpawn
{
    public EnemyKind kind;
    public List<GameObject> kindOfEnemyList = new();
    public bool useRandomPos = true;
    [Min(1)]public int waveMaxSpawnCount = 10;
    public int randomSpawnProbability;
    [Min(0)]public int weight;
    [HideInInspector]public int randomSpawnMin;
    [HideInInspector]public int randomSpawnMax;

    public GameObject RandomPick()
    {
        if(kindOfEnemyList == null || kindOfEnemyList.Count == 0) 
            throw new ArgumentException("kindOfEnemyList cannot be null or empty");

        return kindOfEnemyList[Random.Range(0, kindOfEnemyList.Count)];
    }
}

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    [Header("Settings")] 
    public bool isTest = false;
    public List<EnemySpawn> enemySpawnList = new();
    public GameObject finalBossPrefab;
    public float spawnInterval = 1.5f;
    public float waveInterval = 2f;

    [Space(15)] 
    public Vector3 finalBossGeneratePosition;
    
    [Space(15)]
    public List<SpawnPosition> SpawnPositionsList = new();
    
    private readonly Dictionary<EnemyKind, int> WaveRemaining = new();
    private readonly List<SpawnPositionType> LockedEdge = new();
    private List<int> prefixWeight = new();
    private WaitForSeconds waitSpawn;
    private WaitForSeconds waitWave;


    public override void Awake()
    {
        base.Awake();
        
        if (isTest)
        {
            Debug.LogWarning("Test Mode : Not Spawn Enemy");
            return;
        }

        if (MainGameManager.Instance.isHardMode)
        {
            spawnInterval = 0.5f;
            waveInterval = 1;
        }
        waitSpawn = new WaitForSeconds(spawnInterval);
        waitWave = new WaitForSeconds(waveInterval);
        PrefixWeight();
        StartCoroutine(SpawnEnemy());
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepare += OnBossEventPrepare; // Stop spawn enemy
        EventHandler.FinalBossDeadEventDone += OnFinalBossDeadEventDone; // Start spawn enemy
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= OnBossEventPrepare;
        EventHandler.FinalBossDeadEventDone -= OnFinalBossDeadEventDone;
    }

    private void OnBossEventPrepare()
    {
        StopAllCoroutines();
    }

    private void OnFinalBossDeadEventDone()
    {
        StartCoroutine(SpawnEnemy());
    }

    #endregion

    public void FinalBossGenerate()
    {
        Instantiate(finalBossPrefab, finalBossGeneratePosition, Quaternion.Euler(0, 0, 90));
    }

    IEnumerator SpawnEnemy()
    {
        
        while (true)
        {
            // 1. Choose the Spawn Position
            // 2. Choose the kindEnemyList
            // 3. Choose the Enemy
            
            ResetWaveQuota();
            int batchCount = Random.Range(1, 5);
            
            for (int i = 0; i < batchCount; i++)
            {
                var posInfo = RandomSpawnPosition();
                var enemyInfo = WeightRandomPick(enemySpawnList, enemy => enemy.weight, prefixWeight);

                if (WaveRemaining[enemyInfo.kind] > 0) WaveRemaining[enemyInfo.kind]--;
                else continue;

                if (enemyInfo.kind == EnemyKind.Wall)
                {
                    EventHandler.CallDangerousWallSpawn(posInfo.type);
                    OnDangerousWallSpawned(posInfo.type);
                    yield return new WaitForSeconds(3);
                }

                SpawnEnemy(enemyInfo, posInfo);
                yield return waitSpawn;
            }
            yield return waitWave;
        }
    }

    private void ResetWaveQuota()
    {
        WaveRemaining.Clear();
        foreach (var e in enemySpawnList)
            WaveRemaining[e.kind] = e.waveMaxSpawnCount;
    }
    
    private void PrefixWeight()
    {
        prefixWeight.Clear();
        int sum = 0;
        foreach (var enemy in enemySpawnList)
        {
            sum += enemy.weight;
            prefixWeight.Add(sum);
        }
    }

    private static T WeightRandomPick<T>(List<T> list, Func<T, int> weightSelector, List<int> weightPrefix)
    {
        if (list.Count == 0) throw new ArgumentException("List cannot be empty");
        var sum = list.Sum(weightSelector);

        var randomValue = Random.Range(0, sum);
        for (int i = 0; i < weightPrefix.Count; i++)
            if (randomValue < weightPrefix[i])
                return list[i];
        
        return list[^1];// Impossible;
    }
    
    private SpawnPosition RandomSpawnPosition()
    {
        var list = SpawnPositionsList
            .Where(dir => !LockedEdge.Contains(dir.type))
            .ToList();

        return list[Random.Range(0, list.Count)];
    }

    private void SpawnEnemy(EnemySpawn enemyData, SpawnPosition posInfo)
    {
        var prefab = enemyData.RandomPick();
        var x = new Vector2(posInfo.minSpawnRange.x, posInfo.maxSpawnRange.x);
        var y = new Vector2(posInfo.minSpawnRange.y, posInfo.maxSpawnRange.y);
        var pos = enemyData.useRandomPos
            ? new Vector3(Random.Range(x.x, x.y), Random.Range(y.x, y.y))
            : new Vector3((x.x + x.y) / 2, (y.x + y.y) / 2);
        
        var enemy = Instantiate(prefab, pos, posInfo.spawnRotation) as GameObject;
        
        if(enemy.TryGetComponent<DangerousWallController>(out var wallController))
            wallController.Initialize(Opposite(posInfo.type));
    }

    private void OnDangerousWallSpawned(SpawnPositionType from)
    {
        LockedEdge.Add(Opposite(from)); 
        LockedEdge.Add(from);
    }

    public void OnDangerousWallArrived(SpawnPositionType edge)
    {
        LockedEdge.Remove(Opposite(edge));
        LockedEdge.Remove(edge); 
    }

    private SpawnPositionType Opposite(SpawnPositionType t) => t switch
    {
        SpawnPositionType.Up => SpawnPositionType.Down,
        SpawnPositionType.Down => SpawnPositionType.Up,
        SpawnPositionType.Left => SpawnPositionType.Right,
        SpawnPositionType.Right => SpawnPositionType.Left,
        _ => t
    };
}