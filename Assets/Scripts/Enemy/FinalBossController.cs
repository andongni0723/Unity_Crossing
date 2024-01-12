using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Laser
{
    public EnemyLaserWeapon LaserWeapon;
    public GameObject LaserWeaponSpriteObject;
}

public class FinalBossController : EnemyController
{
    bool isPrepareDone = false;
    bool isShieldPlay = false;
    bool isAttack = false;
    int attackSkillOrder = 0;
    
    int finalBulletShootCount = 0;
    float forwardLaserStartAngle = 0;
    float forwardLaserTargetAngle = 0;

    [Header("Components")] 
    public Laser forwardLaser;
    public Laser backLaser;

    public GameObject shield;
    
    public GameObject skillBulletWarning;
    
    [Header("Setting")]
    public float startDoubleLaserRotateTime = 5;
    private float doubleLaserRotateTime;
    private int finalBulletEachColumnBulletCount = 3;
    private WaitForSeconds finalBulletWaveShootTime = new WaitForSeconds(1);
    private WaitForSeconds finalBulletShootTime = new WaitForSeconds(0.02f);

    [Space(15)] 
    public GameObject finalBossLaserEnemyPrefab;
    public Vector3 finalLaserEnemyPos1;
    public Vector3 finalLaserEnemyPos2;
    public Vector3 finalLaserEnemyPos3;
    private int finalLaserEnemyDeadCount = 0;
    private bool isAllFinalLaserEnemyDead = false;



    protected void Awake()
    {
        StartCoroutine(enemyHealth.GiveEffect(EffectStatus.Invincible, 5));
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepareDone += OnBossEventPrepareDone; // Check isPrepareDone
        EventHandler.FinalBossLaserEnemyDead += OnFinalBossLaserEnemyDead; // Check All FinalBossLaserEnemy is Dead
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepareDone -= OnBossEventPrepareDone;
        EventHandler.FinalBossLaserEnemyDead -= OnFinalBossLaserEnemyDead;
    }

    private void OnFinalBossLaserEnemyDead()
    {
        finalBulletShootCount++;
        enemyHealth.TakeRealDamage(3);

        if (finalBulletShootCount == 3)
        {
            finalBulletShootCount = 0;
            isAllFinalLaserEnemyDead = true;
        }
    }

    private void OnBossEventPrepareDone()
    {
        isPrepareDone = true;
    }

    #endregion

    protected override void MoveAction() { }
    protected override void ThinkingAction() { }

    protected override void AttackAction()
    {
        if(isPrepareDone && !isAttack)
        {
            switch (attackSkillOrder)
            {
                case 0:
                    ForwardLaserBuff();
                    StartCoroutine(ForwardLaserAttack());
                    attackSkillOrder++;
                    break;
                
                case 1:
                    FinalBulletBuff();
                    StartCoroutine(FinalBulletAttack());
                    // Add order in FinalBulletBuff
                    break;
                
                case 2:
                    DoubleLaserBuff();
                    StartCoroutine(DoubleLaserRotate());
                    attackSkillOrder++;
                    break;
                
                case 3:
                    FinalBossLaserEnemyBuff();
                    StartCoroutine(FinalBossLaserEnemy());
                    attackSkillOrder = 0;
                    break;

            }
        }
        
        if(isPrepareDone && !isShieldPlay)
        {
            StartCoroutine(Shield());
        }
        
    }

    #region AttackBuff
    private void ForwardLaserBuff()
    {
        forwardLaser.LaserWeapon.fireTime = 5;
        forwardLaser.LaserWeapon.accumulateTime = Mathf.Max(3 - GameManager.Instance.finalBossAppearCount, 0);
        Debug.Log(forwardLaser.LaserWeapon.accumulateTime);
        
        
        int rand = Random.Range(0, 2);
        forwardLaserStartAngle = rand == 0 ? -180 : 0;
        forwardLaserTargetAngle = rand == 0 ? 0 : -180;
    }
        
    private void FinalBulletBuff()
    {
        int maxShootCount = GameManager.Instance.finalBossAppearCount;
        
        // Hard Mode
        if (MainGameManager.Instance.isHardMode)
        {
            maxShootCount *= 2;
            finalBulletWaveShootTime = new WaitForSeconds(0f);
            finalBulletShootTime = new WaitForSeconds(0.01f);
            finalBulletEachColumnBulletCount = 5;
        }
        // Shoot more time
        if (finalBulletShootCount > maxShootCount)
        {
            finalBulletShootCount = 0;
            attackSkillOrder++;
        }
        else
        {
            finalBulletShootCount++;
        } 
    }
    private void DoubleLaserBuff()
    {
        forwardLaser.LaserWeapon.accumulateTime = 3f;
        backLaser.LaserWeapon.accumulateTime = 3f;

        // Rotate more time 
        doubleLaserRotateTime = startDoubleLaserRotateTime + GameManager.Instance.finalBossAppearCount * 2;
        forwardLaser.LaserWeapon.fireTime = doubleLaserRotateTime;
        backLaser.LaserWeapon.fireTime = doubleLaserRotateTime;
    }

    private void FinalBossLaserEnemyBuff()
    {
        
    }
    
    #endregion

    #region SkillAction
    IEnumerator Shield()
    {
        isShieldPlay = true;
        yield return new WaitForSeconds(5);
        
        shield.SetActive(true);
        yield return new WaitForSeconds(5);
        shield.SetActive(false);
        yield return new WaitForSeconds(5);
        isShieldPlay = false;
    }
    
    IEnumerator ForwardLaserAttack()
    {
        isAttack = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotateQuaternion(Quaternion.Euler(0, 0, forwardLaserStartAngle),
            0.4f).OnComplete(() =>
        {
            forwardLaser.LaserWeaponSpriteObject.SetActive(true);
            forwardLaser.LaserWeapon.Shoot();
        }));
        sequence.AppendInterval(forwardLaser.LaserWeapon.accumulateTime);
        
        sequence.Append(transform.DORotateQuaternion(Quaternion.Euler(0, 0, forwardLaserTargetAngle),
            forwardLaser.LaserWeapon.fireTime));
        sequence.AppendInterval(3);
        sequence.OnComplete(() =>
        {
            forwardLaser.LaserWeaponSpriteObject.SetActive(false);
            isAttack = false;
        });
        
        yield return null;
    }

    IEnumerator FinalBulletAttack()
    {
        isAttack = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        
        for (int i = -10; i < 10; i++)
        {
            for (int j = 0; j < finalBulletEachColumnBulletCount; j++)
            {
                Instantiate(skillBulletWarning, new Vector3(i, Random.Range(-5, 5), 0), Quaternion.identity);
                yield return finalBulletShootTime;
            }
        }

        yield return finalBulletWaveShootTime;
        isAttack = false;
        yield return null;
    }

    IEnumerator DoubleLaserRotate()
    {
        isAttack = true;
        // Laser Shoot
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotateQuaternion(Quaternion.Euler(0, 0, -180), 0.4f).OnComplete(() =>
        {
            forwardLaser.LaserWeaponSpriteObject.SetActive(true);
            backLaser.LaserWeaponSpriteObject.SetActive(true);
            forwardLaser.LaserWeapon.Shoot();
            backLaser.LaserWeapon.Shoot();
        }));

        // Rotate
        yield return new WaitForSeconds(forwardLaser.LaserWeapon.accumulateTime);
        float angle = 1.5f;
        for (int i = 0; i < forwardLaser.LaserWeapon.fireTime * 50; i++)
        {
            transform.Rotate(Vector3.forward * angle); 
            yield return new WaitForSeconds(0.02f);
        }
        
        // Done
        yield return new WaitForSeconds(1);
        forwardLaser.LaserWeaponSpriteObject.SetActive(false);
        backLaser.LaserWeaponSpriteObject.SetActive(false);
        isAttack = false;
    }

    IEnumerator FinalBossLaserEnemy()
    {
        isAttack = true;
        Instantiate(finalBossLaserEnemyPrefab, finalLaserEnemyPos1, Quaternion.identity);
        Instantiate(finalBossLaserEnemyPrefab, finalLaserEnemyPos2, Quaternion.identity);
        Instantiate(finalBossLaserEnemyPrefab, finalLaserEnemyPos3, Quaternion.identity);

        // StartCoroutine(enemyHealth.GiveEffect(EffectStatus.Invincible, 99999));
        enemyHealth.GivePermanentEffect(EffectStatus.Invincible);
        isAllFinalLaserEnemyDead = false;

        Debug.Log("Before");
        yield return new WaitUntil(() => isAllFinalLaserEnemyDead);
        Debug.Log("After");
        
        enemyHealth.ClearEffect();
        
        isAttack = false;
        yield return null;
    }
    
    #endregion
}

