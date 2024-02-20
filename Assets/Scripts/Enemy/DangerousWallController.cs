using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousWallController : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 3;
    
    private Rigidbody2D _rb;

    private void Awake()
    {
        Destroy(gameObject, 15);
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.right * speed;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && AttackTimerCheck())
        {
            other.GetComponent<PlayerHealth>().TakeDamage(1);
            AttackTimerStart();
        }
    }

    #region Attact Timer

    private float _attackTimer;
    private float _attackTimerMax = 1f;
    private void AttackTimerStart() => _attackTimer = _attackTimerMax;
    private bool AttackTimerCheck() => _attackTimer <= 0;
    private void AttackTimer()
    {
        if (_attackTimer >= 0)
            _attackTimer -= Time.deltaTime;
    }

    #endregion
}
