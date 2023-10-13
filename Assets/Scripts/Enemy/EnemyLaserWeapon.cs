using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyLaserWeapon : MonoBehaviour
{
    [Header("Component")] 
    public ParticleSystem accumulateVFX;
    public ParticleSystem warmingVFX;
    public ParticleSystem fireVFX;
    public ParticleSystem shootingUpVFX;
    public ParticleSystem shootingDownVFX;
    
    public GameObject laserObject;


    [Header("Settings")] 
    public float accumulateTime = 3;
    public float fireTime = 5;
    public bool isPlay;
    
    [Space(10)]
    public float minLaserScaleY = 0.6f;
    public float maxLaserScaleY = 1.2f;
    public float laserScaleDuration = 0.5f;

    private void Awake()
    {
        var accumulateVFXMain = accumulateVFX.main;
        accumulateVFXMain.duration = accumulateTime;
        
        var warmingVFXMain = warmingVFX.main;
        warmingVFXMain.duration = accumulateTime;
        
        var fireVFXMain = fireVFX.main;
        fireVFXMain.duration = fireTime;
        
        var shootingUpVFXMain = shootingUpVFX.main;
        shootingUpVFXMain.duration = fireTime;
        
        var shootingDownVFXMain = shootingDownVFX.main;
        shootingDownVFXMain.duration = fireTime;
    }
    
    public void Shoot()
    {
        StartCoroutine(ExecuteShootAction());
    }

    
    IEnumerator ExecuteShootAction()
    {
        isPlay = true;
        accumulateVFX.Play();
        warmingVFX.Play();
        yield return new WaitForSeconds(accumulateTime);
        
        LaserShoot();
        fireVFX.Play();
        shootingUpVFX.Play();
        shootingDownVFX.Play();
        yield return new WaitForSeconds(fireTime);
        
        isPlay = false;
    }
    
    private void LaserShoot()
    {
        laserObject.transform.localScale = new Vector3(laserObject.transform.localScale.x, 0, 1);
        laserObject.SetActive(true);

        laserObject.transform.DOScaleY(maxLaserScaleY, 0.1f);
        
        ShootAnimation();
    }

    private void ShootAnimation()
    {
        Sequence laserSequence = DOTween.Sequence();
        
        // tween the laser scale y
        laserSequence.Append(laserObject.transform.DOScaleY(minLaserScaleY, laserScaleDuration).SetEase(Ease.InOutSine));
        laserSequence.Append(laserObject.transform.DOScaleY(maxLaserScaleY, laserScaleDuration).SetEase(Ease.InOutSine));
        laserSequence.OnComplete(() =>
        {
            if(isPlay)
                ShootAnimation();
            else
                laserObject.SetActive(false);
        });
    }
}
