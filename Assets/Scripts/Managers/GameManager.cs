using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentScore = 0;
    public int bossEventTargetScore = 2;
    public bool isFinalBossAlive = false;

    [Header("Components")] 
    public TextMeshProUGUI scoreText;

    [Header("Setting")] 
    public Vector3 scoreScaleAddVFX;

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
}
