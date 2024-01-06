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

    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(enemyHealth.GiveEffect(EffectStatus.Invincible, 5));
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepareDone += OnBossEventPrepareDone; // Check isPrepareDone
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepareDone -= OnBossEventPrepareDone;
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
                    break;
                case 2:
                    DoubleLaserBuff();
                    StartCoroutine(DoubleLaserRotate());
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
        // Shoot more time
        if (finalBulletShootCount > GameManager.Instance.finalBossAppearCount)
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

        Debug.Log(forwardLaser.LaserWeapon.accumulateTime);
        Debug.Log(backLaser.LaserWeapon.accumulateTime);
        
        // Rotate more time 
        doubleLaserRotateTime = startDoubleLaserRotateTime + GameManager.Instance.finalBossAppearCount * 2;
        forwardLaser.LaserWeapon.fireTime = doubleLaserRotateTime;
        backLaser.LaserWeapon.fireTime = doubleLaserRotateTime;
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
        
        for (int i = -10; i < 8; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Instantiate(skillBulletWarning, new Vector3(i, Random.Range(-5, 5), 0), Quaternion.identity);
                yield return new WaitForSeconds(0.02f);
            }
        }

        yield return new WaitForSeconds(1);
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
    
    #endregion
}

