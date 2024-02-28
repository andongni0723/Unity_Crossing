using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    private PlayerInputControls _controls;
    private Rigidbody2D _rb;
    private PlayerFire _playerFire;
    
    private Vector2 _moveDirection;
    private Vector2 _mousePosition;
    [SerializeField]private Vector2 _worldMousePosition;
    public float speed = 5;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerFire = GetComponent<PlayerFire>();
        _controls = new PlayerInputControls();

#if PLATFORM_STANDALONE 
        _controls.Player.Fire.performed += _ => OnFire();
#else // Mobile
        _controls.Player.Fire.performed += _ => MobileOnFire();
#endif
        
        // Debug
        Application.targetFrameRate = 300;
        
        _controls.Enable();
    }

    #region Event

    private void OnEnable()
    {
        _controls.Enable(); 
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    #endregion 
    
    private void FixedUpdate()
    {
        _moveDirection = _controls.Player.Move.ReadValue<Vector2>();
        _mousePosition = _controls.Player.Look.ReadValue<Vector2>();
        _worldMousePosition = Camera.main.ScreenToWorldPoint(_mousePosition);
        
        Movement();
        
#if PLATFORM_STANDALONE
        Look();
#endif
    }

    private void Movement()
    {
        _rb.velocity = Vector2.MoveTowards(_rb.velocity, new Vector2(_moveDirection.x * Time.deltaTime,
            _moveDirection.y * Time.deltaTime).normalized * speed, Time.deltaTime * speed * 5);
    }

    private void Look()
    {
        var angle = Mathf.Atan2(_worldMousePosition.y - transform.position.y, 
            _worldMousePosition.x - transform.position.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void OnFire()
    {
        _playerFire.Fire();
    }

    private void MobileOnFire()
    {
        Look();
        _playerFire.Fire();
    }
    
}
