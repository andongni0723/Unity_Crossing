using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class FinalLaserEnemyController : LaserEnemyController
{
    protected override IEnumerator Start()
    {
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one, 0.5f));

        yield return sequence.WaitForCompletion();
        StartCoroutine(base.Start());
    }

    private void OnDestroy()
    {
        EventHandler.CallFinalBossLaserEnemyDead();
    }

    protected override void AttackAction()
    {
        if (AttackTimerCheck())
        {
            // Rotate
            float angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
                _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            transform.DORotate(new Vector3(0, 0, angle + Random.Range(-5f, 5f)), 0.5f);
        
            // transform.rotation = Quaternion.Euler(0, 0, angle)
            
            // Shoot
            Debug.Log("Shoot");
            _laserWeapon.Shoot();
            AttackTimerStart();
        }
    }
}
