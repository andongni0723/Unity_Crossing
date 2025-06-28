using System;
using UnityEngine;

[Flags]
public enum PlayerMoveDirection
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
}
public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    private PlayerInputControls _controls;
    private Rigidbody2D _rb;
    private PlayerFire _playerFire;
    private Camera _mainCamera;
    
    private Vector2 _moveDirection;
    private Vector2 _mousePosition;
    [SerializeField]private Vector2 _worldMousePosition;
    public float speed = 5;

    public PlayerMoveDirection playerMoveDirection = PlayerMoveDirection.None;
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerFire = GetComponent<PlayerFire>();
        _controls = new PlayerInputControls();
        _mainCamera = Camera.main;

#if PLATFORM_STANDALONE 
        _controls.Player.Fire.performed += _ => OnFire();
#else // Mobile
        _controls.Player.Fire.performed += _ => MobileOnFire();
#endif
        
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
        _worldMousePosition = _mainCamera.ScreenToWorldPoint(_mousePosition);
        DirectionToPlayerMoveDirection();
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
    
    private void DirectionToPlayerMoveDirection()
    {
        playerMoveDirection = PlayerMoveDirection.None;

        // Horizontal 
        playerMoveDirection |= _moveDirection.x switch
        {
            > 0 => PlayerMoveDirection.Right,
            < 0 => PlayerMoveDirection.Left,
            _ => PlayerMoveDirection.None
        };
        
        // Vertical
        playerMoveDirection |= _moveDirection.y switch
        {
            > 0 => PlayerMoveDirection.Up,
            < 0 => PlayerMoveDirection.Down,
            _ => PlayerMoveDirection.None
        };
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
}