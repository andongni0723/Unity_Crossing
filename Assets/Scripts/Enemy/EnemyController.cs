using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Move, Attack, Die
}
public class EnemyController : MonoBehaviour
{
    private State _currentState;

    private GameObject _target;
    private float _targetDistance;

    private EnemyHealth enemyHealth => GetComponent<EnemyHealth>();


    //[Header("Components")] 
    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();

    [Header("Settings")] 
    public float speed = 3;
    public float attackRange = 1;

    private void Awake()
    {
        _target = GameObject.FindWithTag("Player");
    }

    public void Update()
    {
        DataUpdate();
        ExecuteStateAction();
        CheckState();
    }

    private void DataUpdate()
    {
        _targetDistance = Vector2.Distance(transform.position, _target.transform.position);
    }

    private void CheckState()
    {
        if (_targetDistance <= attackRange)
            _currentState = State.Attack;
        
        else if(_targetDistance > attackRange)
            _currentState = State.Move; 
        
        else if(enemyHealth.currentHealth <= 0)
            _currentState = State.Die;
        
        else
            throw new ArgumentOutOfRangeException();
    }

    private void ExecuteStateAction()
    {
        switch (_currentState)
        {
            case State.Move:
                var angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
                    _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        
                transform.rotation = Quaternion.Euler(0, 0, angle);
                
                transform.position += transform.right * (speed * Time.deltaTime);
                
                break;
            
            case State.Attack:
                break;
            case State.Die:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}