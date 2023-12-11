using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _timer;
    [Header("Settings")]
    public string TimerName = "Timer";
    public float TimerMax = 1f;

    /// <summary>
    /// This method is check timer name is same with wanted timer name
    /// </summary>
    /// <param name="wantedTimerName"></param>
    public void TimerSettingCheck(string wantedTimerName)
    {
        if(wantedTimerName != TimerName)
            Debug.LogError($"TimerError: This timer in {gameObject.name} is not you want (TimerName is not same with TimerSettingCheck)");
    }
    public void TimerStart() => _timer = TimerMax;
    
    /// <summary>
    /// This method is return true when timer is 0 or less than 0
    /// </summary>
    /// <returns></returns>
    public bool TimerCheck() => _timer <= 0;
    private void TimerAction()
    {
        if (_timer >= 0)
            _timer -= Time.deltaTime;
    }
    
    private void LateUpdate()
    {
        TimerAction();
    }
}
