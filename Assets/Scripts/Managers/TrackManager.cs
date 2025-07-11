using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrackData
{
    public Vector2 pos;
    public float time;

    public TargetTrackData(Vector2 pos, float time)
    {
        this.pos = pos;
        this.time = time;
    }
}

public class TrackManager : Singleton<TrackManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]

    public Vector2 targetPosition;
    public Vector2 realTargetPosition;
    public GameObject target;
    public bool isFakeTargetPosition { get; private set; }

    private readonly LinkedList<TargetTrackData> _posDeque = new();

    private void OnEnable()
    {
        EventHandler.PlayerCrossing += AddFakeTargetPosition;
    }

    private void OnDisable()
    {
        EventHandler.PlayerCrossing -= AddFakeTargetPosition;
    }

    private void Update()
    {
        realTargetPosition = target.transform.position;
        if (!isFakeTargetPosition)
            targetPosition = realTargetPosition;
    }

    public void AddFakeTargetPosition(Vector3 fakePos)
    {
        if(_posDeque.Count >= 2)
            _posDeque.RemoveLast();
        
        _posDeque.AddLast(new TargetTrackData(fakePos, 2f));
        
        if (!isFakeTargetPosition)
            StartCoroutine(UpdateTargetPosition()); 
    }
    
    private IEnumerator UpdateTargetPosition()
    {
        isFakeTargetPosition = true;
        while (_posDeque.Count > 0)
        {
            var currentData = _posDeque.First!.Value;
            _posDeque.RemoveFirst();
            targetPosition = currentData.pos;
            yield return new WaitForSeconds(currentData.time);
        }

        targetPosition = realTargetPosition;
        isFakeTargetPosition = false;
    }


    public Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(-8f, 8f),  Random.Range(-4f, 4f));
    }
}