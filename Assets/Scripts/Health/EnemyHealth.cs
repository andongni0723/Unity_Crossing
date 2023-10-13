using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : BaseHealth
{
    public int dieAddScore = 1;
    protected override void Die()
    {
        GameManager.Instance.AddScore(dieAddScore);
        base.Die();
    }
}
