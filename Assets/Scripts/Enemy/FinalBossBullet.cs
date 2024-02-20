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
    private new Collider2D collider;

    [Header("Setting")] 
    public float bulletScaleBigTime = 0.5f;
    public float bulletVFXScaleBigTime = 1f;
    public Vector3 bulletScaleBig = new Vector3(1.5f, 1.5f, 1.5f);

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        BulletAnimation();
    }

    private void BulletAnimation()
    {
        bulletSpriteRenderer.transform.localScale = Vector3.zero;
        bulletVFXSpriteRenderer.transform.localScale = Vector3.zero;
        CameraManager.Instance.CameraShake(0.3f, 0.2f);
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);
        CheckHitPlayer();
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(bulletSpriteRenderer.transform.DOScale(1, bulletScaleBigTime));
        sequence.Join(bulletVFXSpriteRenderer.transform.DOScale(bulletScaleBig, bulletVFXScaleBigTime));
        sequence.Join(bulletVFXSpriteRenderer.DOFade(0, bulletVFXScaleBigTime)
            .OnComplete(() => collider.enabled = false));
        sequence.Append(bulletSpriteRenderer.DOFade(0, bulletScaleBigTime));
        sequence.OnComplete(() => Destroy(gameObject));
    }

    private void CheckHitPlayer()
    {
        if(Vector2.Distance(gameObject.transform.position, GameManager.Instance.player.transform.position) <= 1.1f)
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(1);
    }
}
