using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointer : PoolableObject
{
    //[Header("Component")]
    [Header("Settings")] 
    public Vector2 boundaryStart;
    public Vector2 boundaryEnd;
    [Min(0f)] public float margin = 0.2f;
    
    [Header("Debug")]
    private Transform target;
    private float minX, maxX, minY, maxY;
    private bool started = false;

    public void Initialize(Transform _target)
    {
        target = _target;
        started = true;
    }

    private void OnEnable()
    {
        EventHandler.BossEventPrepare += CallBackPool;
    }

    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= CallBackPool;
    }


    private void Awake()
    {
        minX = boundaryStart.x;
        minY = boundaryStart.y;
        maxX = boundaryEnd.x;
        maxY = boundaryEnd.y;
    }

    public void Update()
    {
        if(!started) return;
        if (target == null)
        {
            CallBackPool();
            return;
        }
        Pointer();
    }

    private void Pointer()
    {
        var tPos = target.position;
        bool inside = (tPos.x >= minX && tPos.x <= maxX) &&
                      (tPos.y >= minY && tPos.y <= maxY);

        if (inside)
        {
            CallBackPool();
            return;
        }
        
        float clampedX = Mathf.Clamp(tPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(tPos.y, minY, maxY);
        
        if (Math.Abs(clampedX - minX) < 0.1f) clampedX += margin;
        else if (Math.Abs(clampedX - maxX) < 0.1f) clampedX -= margin;
        if (Math.Abs(clampedY - minY) < 0.1f) clampedY += margin;
        else if (Math.Abs(clampedY - maxY) < 0.1f) clampedY -= margin;
        
        transform.position = new Vector3(clampedX, clampedY, 0f);
        
        var dir = target.transform.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

    }

    private void CallBackPool()
    {
        target = null;
        started = false;
        ReturnToPool();
    }
}
