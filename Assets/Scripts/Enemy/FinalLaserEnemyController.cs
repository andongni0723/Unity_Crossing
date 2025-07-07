using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class FinalLaserEnemyController : LaserEnemyController
{
    private bool _sendDeath;
    
    protected override IEnumerator Initialize()
    {
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one, 0.5f));
        yield return sequence.WaitForCompletion();
    }

    protected override void OnEnable()
    {
        EventHandler.FinalBossDead += CallBackToPool;
        StartCoroutine(Initialize());
    }

    private void OnDisable()
    {
        EventHandler.FinalBossDead -= CallBackToPool;
        _sendDeath = false;
    }

    public void OnDeadEvent()
    {
        if (_sendDeath) return;
        _sendDeath = true;
        EventHandler.CallFinalBossLaserEnemyDead();
    }

    private void CallBackToPool()
    {
        enemyHealth.DieNotEvent();
    }

    protected override void AttackAction()
    {
        if (AttackTimerCheck())
        {
            // Rotate
            float angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
                _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            transform.DORotate(new Vector3(0, 0, angle + Random.Range(-5f, 5f)), 0.5f);
        
            // Shoot
            _laserWeapon.Shoot();
            AttackTimerStart();
        }
    }
}
