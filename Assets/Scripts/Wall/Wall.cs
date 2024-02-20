using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Components")]
    public GameObject VFXPrefab;

    [Header("Setting")] 
    public GameObject PlayerCrossingVFX; // Fire VFX
    public Vector3 VFXEndPosition;
    public float shakeIntensity = 5;
    public float shakeDuration = 0.02f;
    
    public bool playerXCrossing = false;
    public bool playerYCrossing = false;

    private int xCrossingInt;
    private int yCrossingInt;

    private void Awake()
    {
        xCrossingInt = playerXCrossing ? -1 : 1;
        yCrossingInt = playerYCrossing ? -1 : 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Bullet":
                // Play VFX
                WallVFX newVFX = Instantiate(VFXPrefab, transform.position, transform.rotation).GetComponent<WallVFX>();
                CameraManager.Instance.CameraShake(shakeIntensity, shakeDuration);
                newVFX.endPosition = VFXEndPosition;
                break;
            
            case "Player":
                if (WallTimer.Instance.WallTimerCheck())
                {
                    EventHandler.CallPlayerCrossing(other.transform.position);
                    
                    // Play VFX
                    Instantiate(PlayerCrossingVFX, other.transform.position, Quaternion.identity);
                    
                    // Change player position
                    other.transform.position = new Vector3(
                        other.transform.position.x * xCrossingInt,
                        other.transform.position.y * yCrossingInt); 
                    
                    WallTimer.Instance.WallTimerStart();
                }
                break;
        }
    }
}
