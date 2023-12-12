using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillWarning : MonoBehaviour
{
    [Header("Component")]
    public SpriteRenderer warningSpriteRenderer;
    
    [Header("Setting")]
    public GameObject generateObject;
    public float warningTime = 1f;

    private void Awake()
    {
        Warning();
    }

    private void Warning()
    {
        warningSpriteRenderer.transform.localScale = Vector3.zero;
        warningSpriteRenderer.transform.DOScale(1, warningTime).OnComplete(() =>
        {
            Instantiate(generateObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        });
    }
}
