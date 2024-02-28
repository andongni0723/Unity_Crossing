using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileUI : MonoBehaviour
{
    [Header("Component")]
    public GameObject mobileUI;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
#if PLATFORM_STANDALONE
        mobileUI.SetActive(false);
#else // Mobile
        mobileUI.SetActive(true);
#endif
    }
}
