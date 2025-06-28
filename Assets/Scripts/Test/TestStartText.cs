using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestStartText : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    private TMP_Text text;
    private Image image;
    
    [SerializeField]private float _timer;
    public float timerMax = 1f;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        image = GetComponentInChildren<Image>();
        _timer = timerMax;
    }

    private void Update()
    {
        if (_timer>= 0)
        {
            _timer -= Time.deltaTime;
        }
        
        image.fillAmount = 1 - _timer % 1;
        text.text = ((int)_timer).ToString();
    }
}
