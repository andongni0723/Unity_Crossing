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

    [Header("Settings")] 
    public AudioClip openingBGM;
    public AudioClip gameBGM;
    public AudioClip finalBossBGM;
    public AudioClip fireSound;
    public AudioClip hitWallSound;
    public AudioClip hitSound;
    public AudioClip laserAccumulateSound;
    public AudioClip laserSound;


    public void PlaySoundAudio(AudioClip audioClip)
    {
        soundAudioSource.clip = audioClip;
        soundAudioSource.volume = 1;
        soundAudioSource.Play();
    }
    
    public void PlayLaserSoundAudio(AudioClip audioClip)
    {
        laserSoundAudioSource.clip = audioClip;
        laserSoundAudioSource.volume = 1;
        laserSoundAudioSource.Play();
    }
    
    public void PlayBGM(AudioClip audioClip)
    {
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.volume = 1;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    public void BGMFade(float target, float time)
    {
        Sequence bgmFadeSequence = DOTween.Sequence();
        bgmFadeSequence.Append(bgmAudioSource.DOFade(target, time));
        bgmFadeSequence.OnComplete(() =>
        {
            bgmAudioSource.Stop();
            bgmAudioSource.volume = 1;
        });
    }
}
