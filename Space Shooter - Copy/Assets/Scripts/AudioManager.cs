using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _powerUpSoundGameObject;
    private AudioSource _powerUpAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        // get the audio source from the child
        _powerUpAudioSource = _powerUpSoundGameObject.GetComponent<AudioSource>();
        if (_powerUpAudioSource == null){
            Debug.LogError("Power Up Audio Source is NULL");
        }
    }
    public void PlayerPowerUpSoundClip()
    {
        // play the audio for collecting a power up
        _powerUpAudioSource.Play();
    }
}
