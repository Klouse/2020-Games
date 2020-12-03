using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _sfxAudio;
    [SerializeField]
    private AudioSource _backgroundAudio;
    [SerializeField]
    private AudioSource _bulletAudio;
    public AudioClip _backgroundAudioClip;
    // Start is called before the first frame update
    void Start()
    {
        _backgroundAudio.clip = _backgroundAudioClip;
        _backgroundAudio.loop = true;
        _backgroundAudio.Play();
    }
    public void PlayBulletAudioClip(AudioClip clip, bool loop = false){
        _bulletAudio.loop = loop;
        _bulletAudio.clip = clip;
        _bulletAudio.Play();
    }

    public void PlaySFXAudioClip(AudioClip clip){
        _sfxAudio.clip = clip;
        _sfxAudio.Play();
    }


    public void StopBulletAudio(){
        _bulletAudio.Stop();
    }

    internal void PlaySFXAudioClip(object purchaseAudioClip)
    {
        throw new NotImplementedException();
    }
}
