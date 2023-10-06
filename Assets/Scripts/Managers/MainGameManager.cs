using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public void Start()
    {
        // Game Scene
        AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);
    }
}
