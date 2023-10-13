using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyController : EnemyController
{
    //[Header("Laser Component")]
    private EnemyLaserWeapon _laserWeapon => GetComponent<EnemyLaserWeapon>();


    protected override void MoveAction()
    {
        if(_laserWeapon.isPlay)
            base.MoveAction();
    }

    protected override void AttackAction()
    {
        float angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
            _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
         if (AttackTimerCheck())
         {
              _laserWeapon.Shoot();
              AttackTimerStart();
         }
    }
}
