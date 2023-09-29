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

    public void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Instantiate(FireVFXPrefab, fireVFXPoint.position, firePoint.rotation);
    }
}
