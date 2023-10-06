using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : BaseHealth
{
    protected override void Die()
    {
        GameManager.Instance.AddScore(1);
        base.Die();
    }
}
