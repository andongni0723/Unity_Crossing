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
        Instantiate(DestroyVFXPrefab, transform.position, Quaternion.identity);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _collisionCount++;
        if(_collisionCount >= 2)
            Destroy(gameObject);
        
        // Rebound
        Vector2 inVec = transform.position - _point;
        _point = transform.position;
        Vector2 outVec = Vector2.Reflect(inVec, other.contacts[0].normal);
        rb.velocity = outVec.normalized * speed;
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }
}
