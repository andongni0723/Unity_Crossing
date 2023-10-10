using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : BaseHealth
{
    [Header("Components")]
    public TextMeshProUGUI healthText;
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        healthText.text = "";
        for (int i = 0; i < (int)currentHealth; i++)
        {
            healthText.text += ".";   
        }
    }
    
    protected override void Die()
    {
        Debug.Log("Player Die");
    }
}
