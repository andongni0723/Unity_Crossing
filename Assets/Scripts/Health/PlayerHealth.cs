using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : BaseHealth
{
    [Header("Components")]
    public TextMeshProUGUI healthText;

    protected override void Awake()
    {
        base.Awake();
        UpdateHealthText();
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.PlayerCrossing += OnPlayerCrossing; // Give invincible effect
        EventHandler.BossEventPrepare += OnBossEventPrepare; // Add 3 life
    }

    private void OnDisable()
    {
        EventHandler.PlayerCrossing -= OnPlayerCrossing;
        EventHandler.BossEventPrepare -= OnBossEventPrepare;
    }

    private void OnPlayerCrossing(Vector3 pastPosition)
    {
        StartCoroutine(GiveEffect(EffectStatus.Invincible, 1));
    }
    
    private void OnBossEventPrepare()
    {
        currentHealth += 3;
        UpdateHealthText();
    }

    #endregion 
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        UpdateHealthText();

        StartCoroutine(GiveEffect(EffectStatus.Invincible, 1));
        
        EventHandler.CallPlayerHurt();
    }

    private void UpdateHealthText()
    {
        healthText.text = "";
        for (int i = 0; i < (int)currentHealth; i++)
        {
            healthText.text += ".";   
        }
    }
    
    protected override void Die()
    {
        Debug.Log("Player Die");
        EventHandler.CallPlayerDead();
    }
}
