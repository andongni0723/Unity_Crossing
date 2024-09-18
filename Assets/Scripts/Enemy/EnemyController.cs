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
    private Rigidbody2D _rb;
    protected EnemyHealth enemyHealth;

    [Header("Settings")] 
    public float speed = 3;

    public float attackRange = 1;
    public float thinkingTime = 1;
    
    // [Header("Variables")]
    [SerializeField]private State _currentState;

    [SerializeField]protected GameObject _target;
    private float _targetDistance;
    private Vector3 _targetBeforeCrossingPosition;
    
    private bool _isThinking = false;
    [SerializeField]private bool _isControllerEnabled = true;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    protected virtual void Start()
    {
        if(GameManager.Instance != null)
            _target = GameManager.Instance.player;
        
        if(TeachManager.Instance != null)
            _target = TeachManager.Instance.player;
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.BossEventPrepare += OnBossEventPrepare; // Destroy self
        EventHandler.PlayerCrossing += OnPlayerCrossing; // change state to Thinking
        EventHandler.PlayerDead += OnPlayerDead;
    }
    
    private void OnDisable()
    {
        EventHandler.BossEventPrepare -= OnBossEventPrepare;
        EventHandler.PlayerCrossing -= OnPlayerCrossing;
        EventHandler.PlayerDead -= OnPlayerDead;
    }

    private void OnBossEventPrepare()
    {
        Destroy(gameObject);
    }

    private void OnPlayerCrossing(Vector3 pastPosition)
    {
        // if(_isThinking) return;
        
        _targetBeforeCrossingPosition = pastPosition;
        StopAllCoroutines();
        StartCoroutine(ToThinkingState());
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
        yield return new WaitForSeconds(thinkingTime);
        _isThinking = false;
    }

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

            case State.Thinking:
                ThinkingAction();
                break;
            
            case State.Die:
                DieAction();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void MoveAction()
    {
        float angle = Mathf.Atan2(_target.transform.position.y - transform.position.y, 
            _target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        
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

    protected virtual void ThinkingAction()
    {
        if(Vector2.Distance(_targetBeforeCrossingPosition, transform.position) < 0.3f) // if gone 
        {
            return;
        }
        
        //Move to the position before player crossing
        float angle = Mathf.Atan2(_targetBeforeCrossingPosition.y - transform.position.y, 
            _targetBeforeCrossingPosition.x - transform.position.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position += transform.right * (speed * Time.deltaTime);
    }
    
    protected virtual void DieAction()
    {
        Destroy(gameObject);
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