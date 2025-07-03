using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : PoolableObject
{
    private Rigidbody2D rb;
    [SerializeField]private int _collisionCount = 0;
    private Vector3 _point;

    [Header("Components")]
    public GameObject DestroyVFXPrefab;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;
    private new ParticleSystem particleSystem;


    [Header("Settings")]
    public float speed = 10;

    public int damage = 100;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Initialize()
    {
        trailRenderer.Clear();
        BecomeToPlayerBullet();
        _collisionCount = 0;
        rb.velocity = transform.right * speed;
        _point = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _collisionCount++;

        if (other.gameObject.TryGetComponent(typeof(BaseHealth), out var health))
        {
            // Check Collision item can hurt or not
            (health as BaseHealth).TakeDamage(damage);
            
            // Check if bullet is hit enemy when bullet is rebound
            if (GameManager.Instance != null)
            {
                if(_collisionCount == 2 && !other.gameObject.CompareTag("Player") && !GameManager.Instance.isFinalBossAlive)
                    EventHandler.CallAddScoreEvent(1); 
            }
            
            ReturnToPool();
        }
        else if (_collisionCount >= 2)
        {
            // Rebound again
            ReturnToPool();
        }
        else
        {
            // Rebound
            Vector2 inVec = transform.position - _point;
            _point = transform.position;
            Vector2 outVec = Vector2.Reflect(inVec, other.contacts[0].normal);
            rb.velocity = outVec.normalized * speed;
            
            AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitWallSound);

            if (other.gameObject.CompareTag("Enemy"))
                BecomeToEnemyBullet();
            
            if (other.gameObject.CompareTag("FinalBoss"))
                BecomeToFinalBossBullet();
        }
    }

    private void BecomeToEnemyBullet()
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        spriteRenderer.color = Color.red;
        trailRenderer.startColor = Color.red;
        trailRenderer.endColor = Color.red;
        var particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.red;
    }
    
    private void BecomeToFinalBossBullet()
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
        spriteRenderer.color = Color.magenta;
        trailRenderer.startColor = Color.magenta;
        trailRenderer.endColor = Color.magenta;
        var particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.magenta;
    }

    private void BecomeToPlayerBullet()
    {
        gameObject.layer = LayerMask.NameToLayer("Bullet");
        spriteRenderer.color = Color.white;
        trailRenderer.startColor = Color.white;
        trailRenderer.endColor = Color.white;
        var particleSystemMain = particleSystem.main;
        particleSystemMain.startColor = Color.white; 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(typeof(BaseHealth), out var health))
        {
            if ((health as BaseHealth).canDestroyBullet)
                ReturnToPool();
        }
    }

    public override void ReturnToPool()
    {
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitSound);
        Instantiate(DestroyVFXPrefab, transform.position, Quaternion.identity);
        base.ReturnToPool();
    }
}
