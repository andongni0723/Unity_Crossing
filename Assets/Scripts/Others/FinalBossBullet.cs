using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FinalBossBullet : MonoBehaviour
{
    [Header("Component")]
    public SpriteRenderer bulletSpriteRenderer;
    public SpriteRenderer bulletVFXSpriteRenderer;

    [Header("Setting")] 
    public float bulletScaleBigTime = 0.5f;
    public float bulletVFXScaleBigTime = 1f;
    public Vector3 bulletScaleBig = new Vector3(1.5f, 1.5f, 1.5f);

    private void Awake()
    {
        BulletAnimation();
    }

    private void BulletAnimation()
    {
        bulletSpriteRenderer.transform.localScale = Vector3.zero;
        bulletVFXSpriteRenderer.transform.localScale = Vector3.zero;
        CameraManager.Instance.CameraShake(0.3f, 0.2f);
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bulletSpriteRenderer.transform.DOScale(1, bulletScaleBigTime));
        sequence.Join(bulletVFXSpriteRenderer.transform.DOScale(bulletScaleBig, bulletVFXScaleBigTime));
        sequence.Join(bulletVFXSpriteRenderer.DOFade(0, bulletVFXScaleBigTime));
        sequence.Append(bulletSpriteRenderer.DOFade(0, bulletScaleBigTime));
        sequence.OnComplete(() => Destroy(gameObject));
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }
}
