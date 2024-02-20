using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : BaseHealth
{
    [Header("Components")]
    public GameObject destroyVFX;

    [Header("Settings")]
    public int dieAddScore = 1;
    
    protected override void Die()
    {
        if(destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        
        EventHandler.CallAddScoreEvent(dieAddScore); 
        base.Die();
    }
    
    
}
