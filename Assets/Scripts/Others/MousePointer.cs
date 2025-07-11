using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePointer : MonoBehaviour
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        var mousePos = Mouse.current.position.ReadValue();
        
        transform.position = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 2f));
    }
}
