using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    private new CinemachineVirtualCamera camera;
    private CinemachineBasicMultiChannelPerlin noise;
    

    public override void Awake()
    {
        base.Awake();
        camera = GetComponent<CinemachineVirtualCamera>();
        noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void CameraShake(float intensity, float time)
    {
        // 0.5f, 0.2f
        
        noise.m_AmplitudeGain = intensity;
        //noise
        StartCoroutine(TimeToCloseShake(time));
    }
    
    private IEnumerator TimeToCloseShake(float time)
    {
        yield return new WaitForSeconds(time);

        noise.m_AmplitudeGain = 0;
    }
}
