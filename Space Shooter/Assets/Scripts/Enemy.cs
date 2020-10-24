using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private int _lives = 3;
    private float _lowerYBound = -5.56f;
    private float _upperYBound = 6.99f;
    [SerializeField]
    private int _pointValue = 10;
    private Player _player;
    private Animator _anim;
    int deathAnimationTriggerHash = Animator.StringToHash("OnEnemyDeath");
    private BoxCollider2D _enemyBoxCollider2D;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    [SerializeField]
    private AudioClip _enemyLaserAudioClip;
    private AudioSource _enemyAudioSource;
    [SerializeField]
    private GameObject _defaultLaserPrefab;
    [SerializeField]
    private float _fireRate;
    private float _laserNextFire;
    private bool _enemyIsAlive;
    [SerializeField]
    private GameObject _leftWingDamageIndicatorVisual;
    [SerializeField]
    private GameObject _rightWingDamageIndicatorVisual;
    private bool _leftWingDamageEnabled;
    private bool _rightWingDamageEnabled;
    [SerializeField]
    private Color _enemyLaserColor;

    private void Start() {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _enemyBoxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAudioSource = GetComponent<AudioSource>();
        _rightWingDamageEnabled = false;
        _leftWingDamageEnabled = false;
        if (_player == null){
            Debug.LogError("Player reference on Enemy is NULL");
        }
        if (_anim == null){
            Debug.LogError("Animator on Enemy is NULL");
        }
        if (_enemyBoxCollider2D == null){
            Debug.LogError("Enemy 2D Box Collider is NULL");
        }
        if (_enemyAudioSource == null){
            Debug.LogError("Enemy Audio Source is NULL");
        }
        _pointValue = Random.Range(5,15);
        _enemyAudioSource.clip = _explosionAudioClip;
        _enemyIsAlive = true;
    }
    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
        if (Time.time > _laserNextFire && _enemyIsAlive){
            Shoot();
        }
    }

    private void MoveEnemy(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= _lowerYBound){
            transform.position = new Vector3(transform.position.x, _upperYBound, transform.position.z);
        }
    }

    private void Shoot(){
        _laserNextFire = Time.time + (_fireRate/100);
        Vector3 laserPosition = transform.position;
        GameObject enemyLaser = Instantiate(_defaultLaserPrefab,laserPosition,Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].IsEnemyLaser = true;
            lasers[i].GetComponent<SpriteRenderer>().color = _enemyLaserColor;
        }
        _enemyAudioSource.clip = _enemyLaserAudioClip;
        _enemyAudioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player"){
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage();
            }
            TakeDamage();
        }else if (other.tag == "Laser"){
            if (other.gameObject.GetComponent<Laser>().IsEnemyLaser == false){
                TakeDamage();
                Destroy (other.gameObject);
            }
        }
    }

    private void EnableDamageIndicator()
    {
        if (_leftWingDamageEnabled == false && _rightWingDamageEnabled == false){
            int wingToEnable = Random.Range(0,2);
            if (wingToEnable == 0){
                _leftWingDamageIndicatorVisual.SetActive(true);
                _leftWingDamageEnabled = true;
            }else{
                _rightWingDamageIndicatorVisual.SetActive(true);
                _rightWingDamageEnabled = true;
            }
        }else if (_leftWingDamageEnabled && _rightWingDamageEnabled == false){
            _rightWingDamageIndicatorVisual.SetActive(true);
            _rightWingDamageEnabled = true;
        }else if (_rightWingDamageEnabled && _leftWingDamageEnabled == false){
            _leftWingDamageIndicatorVisual.SetActive(true);
            _leftWingDamageEnabled = true;
        }
    }
    private void DisableDamageIndicators(){
        if (_leftWingDamageEnabled)
            _leftWingDamageIndicatorVisual.SetActive(false);

        if (_rightWingDamageEnabled)
            _rightWingDamageIndicatorVisual.SetActive(false);
    }

    private void ExplodeEnemy(){
        _anim.SetTrigger(deathAnimationTriggerHash);
        _enemyBoxCollider2D.enabled = false; // don't give more points after dying
        DisableDamageIndicators();
        StartCoroutine(RapidlyDecelerate());
        _enemyAudioSource.Play();
        Destroy (this.gameObject,2.8f);
    }

    private void TakeDamage(){
        _lives--;
        if (_lives < 1){
            _enemyIsAlive = false;
            _player.addToScore(_pointValue);
            ExplodeEnemy();
        }else{    
            EnableDamageIndicator();
        }
    }

    IEnumerator RapidlyDecelerate(){
        while (_speed > 0.0f){
            _speed -= 0.35f;
            yield return new WaitForSeconds(0.2f);
        }
        _speed = 0f;
    }
}