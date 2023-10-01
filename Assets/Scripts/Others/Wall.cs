using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Components")]
    public GameObject VFXPrefab;

    [Header("Setting")]
    public Vector3 VFXEndPosition;
    public float shakeIntensity = 5;
    public float shakeDuration = 0.02f;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            WallVFX newVFX = Instantiate(VFXPrefab, transform.position, transform.rotation).GetComponent<WallVFX>();
            CameraManager.Instance.CameraShake(shakeIntensity, shakeDuration);
            newVFX.endPosition = VFXEndPosition;
        }
    }
}
