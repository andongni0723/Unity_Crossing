using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HurtVFX : MonoBehaviour
{
    [Header("Components")]
    private ChromaticAberration chromaticAberration;
    public Image HurtImage;
    
    [Header("Settings")]
    public float hurtDuration = 0.5f;
    
    private void Awake()
    {
        chromaticAberration = GameManager.Instance.mainVolume.profile.components[1] as ChromaticAberration;
    }

    #region Event

    private void OnEnable()
    {
        EventHandler.PlayerHurt += OnPlayerHurt;
    }

    private void OnDisable()
    {
        EventHandler.PlayerHurt -= OnPlayerHurt;
    }

    private void OnPlayerHurt()
    {
        HurtAnimation();
    }

    #endregion

    private void HurtAnimation()
    {
        CameraManager.Instance.CameraShake(0.8f, hurtDuration / 2);
        
        Sequence hurtSequence = DOTween.Sequence();
        hurtSequence.Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 1f, hurtDuration / 2));
        hurtSequence.Join(HurtImage.DOFade(0.5f, hurtDuration / 2));
        hurtSequence.Append(DOTween.To(() => chromaticAberration.intensity.value, x => chromaticAberration.intensity.value = x, 0f, hurtDuration / 2));
        hurtSequence.Join(HurtImage.DOFade(0f, hurtDuration / 2));
    }
}
