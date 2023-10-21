using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyController : EnemyController
{
    //[Header("Components")]
    private EnemyLaserWeapon _laserWeapon => GetComponent<EnemyLaserWeapon>();
    
    [Header("Settings")]
    public float attackRotateSpeed = 1;

    protected override void MoveAction()
    {
        transform.position += transform.right * (speed * Time.deltaTime); 
        RotateAction();
    }

    protected override void ThinkingAction()
    { }

    protected override void AttackAction()
    {
        RotateAction();
        
         if (AttackTimerCheck())
         {
              _laserWeapon.Shoot();
              AttackTimerStart();
         }
    }
    
    private void RotateAction()
    {
        if (CheckTargetLeftOrRight() > 0)
        {
            transform.Rotate(0, 0, attackRotateSpeed * Time.deltaTime);
        }
        if (CheckTargetLeftOrRight() < 0)
        {
            transform.Rotate(0, 0, -attackRotateSpeed * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// <para>Use the direction of target - self and the front of self (Vector3.right) to make a cross product and take the Z value.</para>
    /// 用target - self 的方向和self的前方做外積並取Z值
    /// </summary>
    /// <returns>
    /// <para>If it is positive, it means the target is on the left. If it is negative, it means the target is on the right.</para>
    /// 如果為正 代表目標在左方， 如果為負 代表目標在右方。
    /// </returns>
    private float CheckTargetLeftOrRight()
    {
        float cross = Vector3.Cross(transform.right, _target.transform.position - transform.position).z;
        return Mathf.Sign(cross); 
    }
}
