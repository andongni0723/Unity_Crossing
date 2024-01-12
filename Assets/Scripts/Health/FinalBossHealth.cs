using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalBossHealth : EnemyHealth
{
    [Header("Component")] 
    public TextMeshProUGUI healthText ;

    protected override void Awake()
    {
        base.Awake();
        healthText = GameObject.FindWithTag("FinalBossHealth").GetComponent<TextMeshProUGUI>();
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthText.text = (currentHealth / maxHealth * 100).ToString("00") + "%";
        if(currentHealth <= 0)
            healthText.text = "00%";
    }

    public override void TakeRealDamage(float damage)
    {
        base.TakeRealDamage(damage);
        healthText.text = (currentHealth / maxHealth * 100).ToString("00") + "%";
        if(currentHealth <= 0)
            healthText.text = "00%";
    }

    protected override void Die()
    {
        EventHandler.CallBossDead();
        base.Die();
    }
}
