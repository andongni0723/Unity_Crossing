using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static event Action<Vector3> PlayerCrossing;

    public static void CallPlayerCrossing(Vector3 pastPosition)
    {
        PlayerCrossing?.Invoke(pastPosition);
    }
}
