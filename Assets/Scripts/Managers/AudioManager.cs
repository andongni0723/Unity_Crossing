using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Components")] 
    public AudioSource bgmAudioSource;
    public AudioSource soundAudioSource;
    public AudioSource laserSoundAudioSource;
    public AudioSource VoiceAudioSource;

    [Header("Settings")] 
    public float maxVolume = 1f;
    public AudioClip openingBGM;
    public AudioClip gameBGM;
    public AudioClip finalBossBGM;
    public AudioClip fireSound;
    public AudioClip hitWallSound;
    public AudioClip hitSound;
    public AudioClip laserAccumulateSound;
    public AudioClip laserSound;
    public AudioClip addFeedbackSound;
    
    private bool isBGMAudioFade = false;
    
    public void PlaySoundAudio(AudioClip audioClip)
    {
        soundAudioSource.clip = audioClip;
        soundAudioSource.volume = maxVolume;
        soundAudioSource.Play();
    }

    public void PlayVoiceAudio(AudioClip voiceClip)
    {
        VoiceAudioSource.clip = voiceClip;
        VoiceAudioSource.volume = maxVolume;
        VoiceAudioSource.Play();
    }

    public bool CheckVoicePlayDone()
    {
        return !VoiceAudioSource.isPlaying;
    }
    
    public void PlayLaserSoundAudio(AudioClip audioClip)
    {
        laserSoundAudioSource.clip = audioClip;
        laserSoundAudioSource.volume = maxVolume;
        laserSoundAudioSource.Play();
    }
    
    public void PlayBGM(AudioClip audioClip)
    {
        StartCoroutine(PlayBGMCoroutine(audioClip));
    }
    
    IEnumerator PlayBGMCoroutine(AudioClip audioClip)
    {
        yield return new WaitUntil(() => isBGMAudioFade == false); // Wait until BGM fade done
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.volume = maxVolume;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    public void BGMFade(float target, float time)
    {
        isBGMAudioFade = true;
        Sequence bgmFadeSequence = DOTween.Sequence();
        
        bgmFadeSequence.Append(bgmAudioSource.DOFade(target, time));
        bgmFadeSequence.OnComplete(() =>
        {
            bgmAudioSource.Stop();
            bgmAudioSource.volume = maxVolume;
            isBGMAudioFade = false;
        });
    }
}
