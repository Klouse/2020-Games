using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private AudioClip _explosionAudioClip;
    private AudioSource _explosionAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        _explosionAudioSource = GetComponent<AudioSource>();
        _explosionAudioSource.clip = _explosionAudioClip;
        _explosionAudioSource.Play();
        Destroy(this.gameObject,3.0f);
    }
}
