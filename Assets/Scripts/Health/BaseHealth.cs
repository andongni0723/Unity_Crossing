using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 100;
    [Range(0, 1)] public float defense = 0;
    public float currentHealth;
    public bool canDestroyBullet = true;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage * (1 - defense);
        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
