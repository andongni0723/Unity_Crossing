using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : BaseHealth
{
    [Header("Components")] private EnemyController _controller;
    public GameObject destroyVFX;

    [Header("Settings")] 
    public UnityEvent OnDeath;
    public int dieAddScore = 1;
    
    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyController>();
    }
    
    protected override void Die()
    {
        if(destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        
        EventHandler.CallAddScoreEvent(dieAddScore); 
        OnDeath?.Invoke();
        _controller.ReturnToPool();
    }
}
