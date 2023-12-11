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
    }
}
