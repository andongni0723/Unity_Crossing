using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int currentScore = 0;

    [Header("Components")] 
    public TextMeshProUGUI scoreText;
    
    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = currentScore.ToString();
    }
}
