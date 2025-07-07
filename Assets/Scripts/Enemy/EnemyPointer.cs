using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyPointer : PoolableObject
{
    //[Header("Component")]
    [Header("Settings")] 
    public Vector2 boundaryStart;
    public Vector2 boundaryEnd;
    [Min(0f)] public float margin = 0.2f;
    
    [Header("Debug")]
    [SerializeField] private Transform target;
    private float minX, maxX, minY, maxY;
    private bool started = false;
    
    [Header("VFX")]
    public float popDuration   = 0.25f;      // 彈出時間
    public float pulseScale    = 1.2f;       // 呼吸最大倍數
    public float pulseDuration = 0.6f;       // 呼吸一來回時間
    public float shakeAngle    = 8f;         // 抖動角度
    public float shakeDuration = 0.3f;       // 抖動週期

/* 私有 */
    private Sequence _pulseSeq;
    private Tween    _shakeTw;
    private Vector3  _originScale;

    public void Initialize(Transform _target)
    {
        transform.position = new Vector3(100, 100, 100);
        target = _target;
        started = true;
        _originScale = transform.localScale;

        //Bounce In
        transform.localScale = Vector3.zero;
        transform.DOScale(_originScale, popDuration)
            .SetEase(Ease.OutBack);

        // Breathing Zoom
        _pulseSeq?.Kill();
        _pulseSeq = DOTween.Sequence()
            .Append(transform.DOScale(_originScale * pulseScale, pulseDuration / 2))
            .Append(transform.DOScale(_originScale,pulseDuration / 2))
            .SetLoops(-1);

        // Dir Shake
        _shakeTw?.Kill();
        _shakeTw = transform.DOLocalRotate(
                new Vector3(0, 0, shakeAngle),
                shakeDuration/2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative()
            .SetEase(Ease.InOutSine);
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

        var fadeOut = DOTween.Sequence();
        fadeOut.Append(transform.DOScale(0, popDuration));
        fadeOut.OnComplete(() =>
        {
            _pulseSeq?.Kill();
            _shakeTw?.Kill();
            transform.localScale = _originScale;
            transform.rotation   = Quaternion.identity;
            ReturnToPool();
        });
    }
}
