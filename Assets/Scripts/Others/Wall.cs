using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [Header("Components")]
    public GameObject VFXPrefab;

    [Header("Setting")]
    public Vector3 VFXEndPosition;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            WallVFX newVFX = Instantiate(VFXPrefab, transform.position, transform.rotation).GetComponent<WallVFX>();
            newVFX.endPosition = VFXEndPosition;
        }
    }
}
