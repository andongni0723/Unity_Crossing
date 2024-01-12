using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    [SerializeField]private int _collisionCount = 0;
    private Vector3 _point;

    [Header("Components")]
    public GameObject DestroyVFXPrefab;
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private TrailRenderer trailRenderer => GetComponent<TrailRenderer>();
    private new ParticleSystem particleSystem => GetComponent<ParticleSystem>();


    [Header("Settings")]
    public float speed = 10;

    public int damage = 100;

    private void Start()
    {
        rb.velocity = transform.right * speed;
        _point = transform.position;
    }

    private void OnDestroy()
    {
        AudioManager.Instance.PlaySoundAudio(AudioManager.Instance.hitSound);
        Instantiate(DestroyVFXPrefab, transform.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _collisionCount++;

        if (other.gameObject.TryGetComponent(typeof(BaseHealth), out var health))
        {
            // Check Collision item can hurt or not
            (health as BaseHealth).TakeDamage(damage);
            
            // Check if bullet is hit enemy when bullet is rebound
            if(_collisionCount == 2 && !other.gameObject.CompareTag("Player") && !GameManager.Instance.isFinalBossAlive)
                GameManager.Instance.AddScore(1);
            
            Destroy(gameObject);
        }
        else if (_collisionCount >= 2)
        {
            // Rebound again
            Destroy(gameObject);
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(typeof(BaseHealth), out var health))
        {
            if ((health as BaseHealth).canDestroyBullet)
                Destroy(gameObject);
        }
    }
}
