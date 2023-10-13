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
        Debug.Log(other.gameObject.name);

        if (other.gameObject.TryGetComponent(typeof(BaseHealth), out var health))
        {
            // Check Collision item can hurt or not
            (health as BaseHealth).TakeDamage(damage);
            
            // Check if bullet is hit enemy when bullet is rebound
            if(_collisionCount == 2)
                GameManager.Instance.AddScore(1);
            
            Destroy(gameObject);
        }
        else if (_collisionCount >= 2)
        {
            // Rebound more time
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
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject); 
    }
}
