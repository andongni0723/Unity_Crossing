using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] 
    public PlayerInputControls controls;
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    
    private Vector2 moveDirection;
    private Vector2 mousePosition;
    private Vector2 worldMousePosition;
    public float speed = 5;


    private void Awake()
    {
        controls = new PlayerInputControls();
        
        // Debug
        Application.targetFrameRate = 300;
        
        controls.Enable();
    }

    private void FixedUpdate()
    {
        moveDirection = controls.Player.Move.ReadValue<Vector2>();
        mousePosition = controls.Player.Look.ReadValue<Vector2>();
        worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        Movement();
        Look();
    }

    private void Movement()
    {
        rb.velocity = Vector2.MoveTowards(rb.velocity, new Vector2(moveDirection.x * Time.deltaTime,
            moveDirection.y * Time.deltaTime).normalized * speed, Time.deltaTime * speed * 5);
    }

    private void Look()
    {
        
        float angle = Mathf.Atan2(worldMousePosition.y - transform.position.y, 
            worldMousePosition.x - transform.position.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
