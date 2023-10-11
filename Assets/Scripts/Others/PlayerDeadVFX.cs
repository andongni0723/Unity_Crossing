using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerDeadVFX : MonoBehaviour
{
    [Header("Components")]
    private LensDistortion lensDistortion;
    public CanvasGroup DeadCanvasGroup;
    
    [Header("Settings")]
    public float VFXDuration = 1;
    
    private void Awake()
    {
        lensDistortion = FindObjectOfType<Volume>().profile.components[2] as LensDistortion;
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.PlayerDead += OnPlayerDead;
    }

    private void OnDisable()
    {
        EventHandler.PlayerDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        DeadAnimation();
    }

    #endregion

    private void DeadAnimation()
    {
        AudioManager.Instance.BGMFade(0, VFXDuration);
        
        Time.timeScale = 0.2f;
        Sequence deadSequence = DOTween.Sequence();
        deadSequence.Append(DOTween.To(() => lensDistortion.intensity.value, x => lensDistortion.intensity.value = x, -1, VFXDuration / 2));
        deadSequence.Append(DOTween.To(() => lensDistortion.scale.value, x => lensDistortion.scale.value = (float)x, 0.01, VFXDuration / 2).SetEase(Ease.OutSine));
        deadSequence.Join(DeadCanvasGroup.DOFade(1, VFXDuration / 2));
        //deadSequence.OnComplete(() => Time.timeScale = 0);
    }
}
