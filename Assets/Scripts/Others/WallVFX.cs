using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WallVFX : MonoBehaviour
{
    // [Header("Components")]
    private SpriteRenderer _spriteRenderer => GetComponent<SpriteRenderer>();
    
    [Header("Settings")]
    public Vector3 endPosition;
    public float time = 1;

    private Vector3 _startPosition;
    private void Start()
    {
        Destroy(gameObject, time);
        _startPosition = transform.position;

        StartCoroutine(VFX());
    }

    IEnumerator VFX()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(_startPosition + endPosition, time));
        sequence.Join(_spriteRenderer.DOFade(0, time - 0.3f)); 
        
        yield return null;
    }
}
