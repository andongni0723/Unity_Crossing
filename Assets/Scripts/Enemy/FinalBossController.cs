using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : EnemyController
{
    protected override void Awake()
    {
        base.Awake();
        Debug.Log('g');
        StartCoroutine(enemyHealth.GiveEffect(EffectStatus.Invincible, 5));
    }

    protected override void MoveAction()
    {
    }

    protected override void ThinkingAction()
    {
    }
}
