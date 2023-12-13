using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentScore = 0;
    public int baseBossEventTargetScore = 30;
    [SerializeField] int bossEventTargetScore;
    public bool isFinalBossAlive = false;

    public int finalBossAppearCount = 0;
    

    [Header("Components")] 
    public TextMeshProUGUI scoreText;

    [Header("Setting")] 
    public Vector3 scoreScaleAddVFX;

    #region Event

    private void OnEnable()
    {
        EventHandler.FinalBossDead += OnFinalBossDead; // Update boss data
        EventHandler.FinalBossDeadEventDone += OnFinalBossDeadEventDone; // Update boss Status
    }

    private void OnDisable()
    {
        EventHandler.FinalBossDead -= OnFinalBossDead;
        EventHandler.FinalBossDeadEventDone -= OnFinalBossDeadEventDone;
    }

    private void OnFinalBossDead()
    {
        UpdateBossData();
    }

    private void OnFinalBossDeadEventDone()
    {
        isFinalBossAlive = false;
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        bossEventTargetScore = baseBossEventTargetScore;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = currentScore.ToString();

        Sequence sequence = DOTween.Sequence();
        sequence.Append(scoreText.transform.DOScale(scoreScaleAddVFX, 0.1f));
        sequence.Append(scoreText.transform.DOScale(Vector3.one, 0.1f));
    }

    private void Update()
    {
        if (currentScore >= bossEventTargetScore && !isFinalBossAlive)
        {
            EventHandler.CallBossEventPrepare();
            isFinalBossAlive = true;
        }
    }

    private void UpdateBossData()
    {
        finalBossAppearCount++;
        isFinalBossAlive = false;
        bossEventTargetScore = currentScore + finalBossAppearCount * 10 + 30;
        
        if (EnemySpawnManager.Instance.waitNextSpawnTime <= 0.6)
            EnemySpawnManager.Instance.waitNextSpawnTime = 0.6f;
        else 
            EnemySpawnManager.Instance.waitNextSpawnTime -= 0.3f;
    }
}
