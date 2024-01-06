using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : Singleton<GameManager>
{
    public int currentScore = 0;
    public int baseBossEventTargetScore = 30; 
    private int bossEventTargetScore;
    public bool isFinalBossAlive = false;

    public int finalBossAppearCount = 0;


    [Header("Components")] 
    public GameObject player;
    public TextMeshProUGUI scoreText;
    public Volume mainVolume;
    public Volume finalBossVolume;

    [Header("Setting")] 
    public Vector3 scoreScaleAddVFX;

    public override void Awake()
    {
        base.Awake();
        AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);
        player = GameObject.FindGameObjectWithTag("Player");
        bossEventTargetScore = baseBossEventTargetScore;
    }

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
        
        EnemySpawnManager.Instance.waitNextSpawnTime = 
            Mathf.Max(EnemySpawnManager.Instance.waitNextSpawnTime - 0.3f, 0.3f);
    }
}
