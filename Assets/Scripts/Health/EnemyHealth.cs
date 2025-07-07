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

    
    protected virtual void Awake()
    {
        _controller = GetComponent<EnemyController>();
    }

    public void DieNotEvent()
    {
        if(destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        _controller.ReturnToPool(); 
    }
    
    protected override void Die()
    {
        EventHandler.CallAddScoreEvent(dieAddScore);
        OnDeath?.Invoke();
        DieNotEvent();
    }
}
