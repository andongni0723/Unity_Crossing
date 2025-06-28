using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerousWallController : MonoBehaviour
{
    [Header("Settings")] 
    public SpawnPositionType dir;
    public float speed = 3;
    
    private Rigidbody2D _rb;
    private bool isAttack;
    private Vector3 arrivedPos;

    public void Initialize(SpawnPositionType _dir)
    {
        dir = _dir;
        arrivedPos = GetArrivePosition();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.right * speed;
        
        if (Vector3.Distance(transform.position, arrivedPos) < 0.1f)
        {
            EnemySpawnManager.Instance.OnDangerousWallArrived(dir);
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttack || !other.CompareTag("Player")) return;
        other.GetComponent<PlayerHealth>().TakeDamage(1);
        isAttack = true;
    }

    private Vector3 GetArrivePosition() => dir switch
    {
        SpawnPositionType.Left => new Vector3(-10, 0, 0),
        SpawnPositionType.Right => new Vector3(10, 0, 0),
        SpawnPositionType.Up => new Vector3(0, 7, 0),
        SpawnPositionType.Down => new Vector3(0, -7, 0),
        _ => Vector3.zero
    };
}
