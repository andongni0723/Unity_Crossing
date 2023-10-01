using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("Component")]
    public GameObject bulletPrefab;
    public GameObject FireVFXPrefab;
    public Transform firePoint;
    public Transform fireVFXPoint;
    
    [Header("Settings")]
    public float shakeIntensity = 5;
    public float shakeDuration = 0.02f;

    public void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Instantiate(FireVFXPrefab, fireVFXPoint.position, firePoint.rotation);
        CameraManager.Instance.CameraShake(shakeIntensity, shakeDuration);
    }
}
