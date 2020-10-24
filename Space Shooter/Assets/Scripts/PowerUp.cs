using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float _speed = 3.0f;
    private float _lowerYBound = -5.56f;
    [SerializeField]
    private string _powerUpName;
    // Start is called before the first frame update
    // Power Up IDs:
    // 0 = Triple Shot
    // 1 = Speed
    // 2 = Shield
    // 3 = Focus Shot
    [SerializeField]
    private int powerUpID;
    private AudioManager _audioManager;

    private void Start() {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null){
            Debug.LogError("Power Up Audio Manager reference is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < _lowerYBound){
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player"){
            Player player = other.GetComponent<Player>();
            if (player != null){
                switch (powerUpID){
                    case 0:
                        player.TripleShotActive(powerUpID);
                        break;
                    case 1:
                        player.SpeedActive(powerUpID);
                        break;
                    case 2:
                        player.ShieldActive(powerUpID);
                        break;
                    case 3:
                        player.FocusShotActive(powerUpID);
                        break;
                    default:
                        Debug.Log("Default Value for Power Up");
                        break;
                }
            }
            _audioManager.PlayerPowerUpSoundClip();
            Destroy(gameObject);
        }
    }
}
