using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWarningUI : MonoBehaviour
{
    public SpawnPositionType UIPositionType;
    public GameObject WarningSpriteGameObject;
    public float animationStopTime = 1f;
    private WaitForSeconds animationWait => new WaitForSeconds(animationStopTime);


    private void Awake() => WarningSpriteGameObject.SetActive(false);

    #region Event
    private void OnEnable()
    {
        EventHandler.DangerousWallSpawn += OnDangerousWallSpawn;
    }

    private void OnDisable()
    {
        EventHandler.DangerousWallSpawn -= OnDangerousWallSpawn;
    }

    private void OnDangerousWallSpawn(SpawnPositionType type)
    {
        if (UIPositionType == type)
        {
            StartCoroutine(WarningAnimation());
        }
    }
    #endregion

    IEnumerator WarningAnimation()
    {
        for (int i = 0; i < 3; i++)
        {
            WarningSpriteGameObject.SetActive(!WarningSpriteGameObject.activeSelf);
            yield return animationWait;
        }
        
        WarningSpriteGameObject.SetActive(false);
        yield return null;
    }
}
