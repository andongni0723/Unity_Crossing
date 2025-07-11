using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyController : EnemyController
{
    //[Header("Components")]
    protected Camera mainCamera;
    protected EnemyLaserWeapon _laserWeapon;
    
    [Header("Settings")]
    public float attackRotateSpeed = 1;

    private Vector2 _randomPos;

    protected override void Awake()
    {
        base.Awake();
        _laserWeapon = GetComponent<EnemyLaserWeapon>(); 
        mainCamera = Camera.main;
    }

    protected virtual IEnumerator Initialize()
    {
        var t = attackRange;
        _laserWeapon.isPlay = false;
        attackRange = 0;
        yield return new WaitForSeconds(1.5f);
        attackRange = t;
        _randomPos = TrackManager.Instance.GetRandomPosition();
    }

    protected override void OnEnable()
    {
        StartCoroutine(Initialize());
        base.OnEnable();
    }

    bool IsInCameraView(Vector3 worldPos)
    {
        var viewport = mainCamera.WorldToViewportPoint(worldPos);

        return viewport.x is >= 0f and <= 1f &&
               viewport.y is >= 0f and <= 1f;
    }

    protected override void MoveAction()
    {
        if (!_laserWeapon.isPlay)
        {
            var angle = Mathf.Atan2(_randomPos.y - transform.position.y,
                _randomPos.x - transform.position.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position += transform.right * (speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _randomPos) < 0.2f)
                _randomPos = TrackManager.Instance.GetRandomPosition();
        }
        else
            RotateAction();
    }

    protected override bool OtherAttackCheck()
    {
        return IsInCameraView(transform.position);
    }

    protected override void AttackAction()
    {
        RotateAction();

        if (!AttackTimerCheck()) return;
             
        _laserWeapon.Shoot();
        AttackTimerStart();
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
    ///
    
    // 用target - self 的方向和self的前方做外積並取Z值
    // 如果為正 代表目標在左方， 如果為負 代表目標在右方。
    private float CheckTargetLeftOrRight()
    {
        float cross = Vector3.Cross(transform.right, _target.transform.position - transform.position).z;
        return Mathf.Sign(cross); 
    }
}
