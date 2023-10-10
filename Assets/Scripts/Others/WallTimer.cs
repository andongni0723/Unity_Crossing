using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTimer : Singleton<WallTimer>
{
    public float wallTimer = 0;
    public float wallTimerMax = 0.5f;

    public void WallTimerStart() => wallTimer = wallTimerMax;
    public bool WallTimerCheck() => wallTimer <= 0;
    private void Update()
    {
        if (wallTimer >= 0)
            wallTimer -= Time.deltaTime;
    }
}
