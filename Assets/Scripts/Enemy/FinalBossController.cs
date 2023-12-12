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
    //public SpriteRenderer LaserWeaponSpriteRenderer; 
}

public class FinalBossController : EnemyController
{
    bool isPrepareDone = false;
    bool isShieldPlay = false;
    [SerializeField]bool isAttack = false;
    [SerializeField]int attackSkillOrder = 0;

    [Header("Components")] 
    public Laser forwardLaser;
    public Laser backLaser;

    public GameObject shield;
    
    public GameObject skillBulletWarning;

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

    protected override void AttackAction()
    {
        if(isPrepareDone && !isAttack)
        {
            switch (attackSkillOrder)
            {
                case 0:
                    StartCoroutine(ForwardLaserAttack());
                    attackSkillOrder++;
                    break;
                case 1:
                    StartCoroutine(MoreBulletAttack());
                    attackSkillOrder++;
                    break;
                case 2:
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
    protected override void MoveAction() { }
    protected override void ThinkingAction() { }
    
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
        Debug.Log("FFF");
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotateQuaternion(Quaternion.Euler(0, 0, -180), 0.4f).OnComplete(() =>
        {
            forwardLaser.LaserWeaponSpriteObject.SetActive(true);
            forwardLaser.LaserWeapon.Shoot();
        }));
        sequence.AppendInterval(forwardLaser.LaserWeapon.accumulateTime);
        sequence.Append(transform.DORotateQuaternion(Quaternion.Euler(0, 0, 0), forwardLaser.LaserWeapon.fireTime));
        sequence.AppendInterval(3);
        sequence.OnComplete(() =>
        {
            forwardLaser.LaserWeaponSpriteObject.SetActive(false);
            isAttack = false;
        });
        
        yield return null;
    }

    IEnumerator MoreBulletAttack()
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
        isAttack = false;
        yield return null;
    }

    IEnumerator DoubleLaserRotate()
    {
        isAttack = true;
        Debug.Log("DDD");
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
        for (int i = 0; i < 360/angle; i++)
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
}

