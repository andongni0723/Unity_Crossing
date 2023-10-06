using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Components")] 
    public AudioSource bgmAudioSource;
    public AudioSource soundAudioSource;

    [Header("Settings")] public AudioClip openingBGM;
    public AudioClip gameBGM;
    public AudioClip fireSound;
    public AudioClip hitWallSound;
    public AudioClip hitSound;


    public void PlaySoundAudio(AudioClip audioClip)
    {
        soundAudioSource.clip = audioClip;
        soundAudioSource.Play();
    }
    
    public void PlayBGM(AudioClip audioClip)
    {
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.Play();
    }
}
