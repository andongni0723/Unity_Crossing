using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Move, Thinking ,Attack, Die
}
public class EnemyController : MonoBehaviour
{
    //[Header("Components")] 
    private Rigidbody2D _rb => GetComponent<Rigidbody2D>();
    private EnemyHealth enemyHealth => GetComponent<EnemyHealth>();

    [Header("Settings")] 
    public float speed = 3;

    public float attackRange = 1;
    
    // [Header("Variables")]
    private State _currentState;

    private GameObject _target;
    private float _targetDistance;
    private Vector3 _targetBeforeCrossingPosition;
    
    private bool _isThinking = false;

    private void Awake()
    {
        _target = GameObject.FindWithTag("Player");
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.PlayerCrossing += OnPlayerCrossing; // change state to Thinking
    }
    
    private void OnDisable()
    {
        EventHandler.PlayerCrossing -= OnPlayerCrossing;
    }
    
    private void OnPlayerCrossing(Vector3 pastPosition)
    {
        _targetBeforeCrossingPosition = pastPosition;
        StartCoroutine(ToThinkingState());
    }

    #endregion
    
    public void LateUpdate()
    {
        if(_target == null) return;
        
        DataUpdate();
        ExecuteStateAction();
        CheckState();
        AttackTimer();
    }

    

    private void DataUpdate()
    {
        _targetDistance = _isThinking? 
            Vector2.Distance(transform.position, _targetBeforeCrossingPosition) : 
            Vector2.Distance(transform.position, _target.transform.position);
    }

    private void CheckState()
    {   
        if(_isThinking)
            _currentState = State.Thinking;
        
        else if (_targetDistance <= attackRange)
            _currentState = State.Attack;
        
        else if(_targetDistance > attackRange)
            _currentState = State.Move; 
        
        else if(enemyHealth.currentHealth <= 0)
            _currentState = State.Die;
        
        else
            throw new ArgumentOutOfRangeException();
    }

    IEnumerator ToThinkingState()
    {
        _isThinking = true;
        yield return new WaitForSeconds(1);
        _isThinking = false;
    }

    private void ExecuteStateAction()
    {
        float angle;
        
        switch (_currentState)
        {
            case State.Move:
                angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
                    _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        
                transform.rotation = Quaternion.Euler(0, 0, angle);
                transform.position += transform.right * (speed * Time.deltaTime);
                break;
            
            case State.Attack:
                if (AttackTimerCheck())
                {
                    _target.GetComponent<BaseHealth>().TakeDamage(1);
                    AttackTimerStart();
                }
                break;
            
            case State.Die:
                break;
            
            case State.Thinking:
                //Move to the position before player crossing
                angle = Mathf.Atan2(_targetBeforeCrossingPosition.y - transform.position.y, 
                    _targetBeforeCrossingPosition.x - transform.position.x) * Mathf.Rad2Deg;
        
                transform.rotation = Quaternion.Euler(0, 0, angle);
                transform.position += transform.right * (speed * Time.deltaTime);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #region AttackTimer
    
    private float _attackTimer;
    private float _attackTimerMax = 1f;
    private void AttackTimerStart() => _attackTimer = _attackTimerMax;
    private bool AttackTimerCheck() => _attackTimer <= 0;
    private void AttackTimer()
    {
        if (_attackTimer >= 0)
            _attackTimer -= Time.deltaTime;
    }
    #endregion
    
    
}