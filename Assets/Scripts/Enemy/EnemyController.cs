using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum State
{
    Move ,Attack, Die, Find
}
public class EnemyController : PoolableObject
{
    //[Header("Components")] 
    private Rigidbody2D _rb;
    protected EnemyHealth enemyHealth;

    [Header("Settings")] 
    public float speed = 3;

    public float attackRange = 1;
    public float thinkingTime = 1;
    
    // [Header("Variables")]
    [SerializeField]private State _currentState;

    [SerializeField]protected GameObject _target;
    private Vector2 _targetPosition;
    private float _targetDistance;
    // private Vector3 _targetBeforeCrossingPosition;
    
    private bool _isThinking;
    [SerializeField]private bool _isControllerEnabled = true;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Initialize()
    {
        var _speed = speed;
        _target = TrackManager.Instance.target;
        speed = 0;
        DOTween.To(() => speed, x => speed = x, _speed, 1); 
    }

    protected virtual void Start()
    {
        _target = TrackManager.Instance.target;
    }

    #region Event

    protected virtual void OnEnable()
    {
        Initialize();
        EventHandler.BossEventPrepare += ReturnToPool; // 
        EventHandler.PlayerDead += OnPlayerDead;
        Debug.Log("On Enable " + gameObject.name);
    }
    
    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= ReturnToPool;
        EventHandler.PlayerDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        _isControllerEnabled = false;
    }

    #endregion
    
    public void LateUpdate()
    {
        if(_target == null || !_isControllerEnabled) return;
        
        DataUpdate();
        ExecuteStateAction();
        CheckState();
        AttackTimer();
    }

    

    private void DataUpdate()
    {
        _targetPosition = TrackManager.Instance.targetPosition;
        _targetDistance = Vector2.Distance(transform.position, _targetPosition);
    }

    private void CheckState()
    {   
        if (!TrackManager.Instance.isFakeTargetPosition && _targetDistance <= attackRange && OtherAttackCheck())
            _currentState = State.Attack;
        
        else if(_targetDistance > attackRange)
            _currentState = State.Move; 
        
        else if (enemyHealth.currentHealth <= 0)
            _currentState = State.Die;

        else
            _currentState = State.Find;
    }

    protected virtual bool OtherAttackCheck() => true;

    private void ExecuteStateAction()
    {
        switch (_currentState)
        {
            case State.Move:
                MoveAction();
                break;
            
            case State.Attack:
                AttackAction();
                break;

            case State.Die:
                DieAction();
                break;
        }
    }

    protected virtual void MoveAction()
    {
        float angle = Mathf.Atan2(_targetPosition.y - transform.position.y, 
            _targetPosition.x - transform.position.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position += transform.right * (speed * Time.deltaTime); 
    }
    
    protected virtual void AttackAction()
    {
        if (AttackTimerCheck())
        {
            _target.GetComponent<BaseHealth>().TakeDamage(1);
            AttackTimerStart();
        }
    }
    
    protected virtual void DieAction()
    {
        ReturnToPool();
    }

    #region AttackTimer
    
    private float _attackTimer;
    public float _attackTimerMax = 1f;
    protected void AttackTimerStart() => _attackTimer = _attackTimerMax;
    protected bool AttackTimerCheck() => _attackTimer <= 0;
    private void AttackTimer()
    {
        if (_attackTimer >= 0)
            _attackTimer -= Time.deltaTime;
    }
    #endregion
    
    
}