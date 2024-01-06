using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : Singleton<MainGameManager>
{
    [Header("Data")]
    public int highScore;
    public int latestScore;
    public void Start()
    {
        // Game Scene
        LoadGameData();
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.PlayerDead += OnPlayerDead;
    }

    private void OnDisable()
    {
        EventHandler.PlayerDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        SaveGameData(GameManager.Instance.currentScore);
    }

    #endregion 

    private void SaveGameData(int score)
    {
        latestScore = score;
        if (latestScore > highScore)
        {
            highScore = latestScore;
        }
        
        PlayerPrefs.SetInt("HighScore", highScore);
        Debug.Log("Save Game Data");
    }
    
    private void LoadGameData()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("Load Game Data");
    }

}
