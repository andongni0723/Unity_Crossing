using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GlitchText : MonoBehaviour
{
    [Header("Component")] 
    private TextMeshProUGUI text;
    
    [Header("Settings")]
    [Range(0, 23)]public int length = 15;
    public float interval = 0.2f;
    public bool autoStart = true;

    [Header("Debug")] 
    private static readonly char[] CHARSET =
        "!@#$%^&*()_+={}|\\?><[]".ToCharArray();

    private char[] buffer;
    private WaitForSeconds wait;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        wait = new WaitForSeconds(interval);
        buffer = new char[length];
        length = Mathf.Clamp(length, 0, buffer.Length);
        if (autoStart)
            StartCoroutine(GlitchLoop());
    }

    private IEnumerator GlitchLoop()
    {
        while (true)
        {
            for (var i = 0; i < length; i++)
                buffer[i] = CHARSET[Random.Range(0, CHARSET.Length)];
            
            text.SetCharArray(buffer, 0, length);
            yield return wait;
        }
    }
}
