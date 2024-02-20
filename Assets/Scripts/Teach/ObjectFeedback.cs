using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFeedback : MonoBehaviour
{
    //[Header("Component")]
    [Header("Settings")]
    public bool isDestroyFeedback = false;
    public int feedbackAmount = 1;
    
    //[Header("Debug")]

    private void OnDestroy()
    {
        if(isDestroyFeedback)
            EventHandler.CallTeachUnitFeedback(feedbackAmount);
    }
}
