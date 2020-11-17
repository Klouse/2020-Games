using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _startingSpeed = 3.5f;

    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _startingLives = 3;
    private float _lowerYBound = -5.56f;
    private float _upperYBound = 6.99f;
    [SerializeField]
    private int _pointValue = 10;
    private Player _player;
    private Animator _anim;
    int deathAnimationTriggerHash = Animator.StringToHash("OnEnemyDeath");
    int enableAnimationTriggerHash = Animator.StringToHash("OnEnemyEnable");
    private BoxCollider2D _enemyBoxCollider2D;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    [SerializeField]
    private AudioClip _enemyLaserAudioClip;
    private AudioSource _enemyAudioSource;
    [SerializeField]
    private float _fireRate;
    private float _laserNextFire;
    [HideInInspector]
    public bool enemyIsAlive;
    [SerializeField]
    private GameObject _leftWingDamageIndicatorVisual;
    [SerializeField]
    private GameObject _rightWingDamageIndicatorVisual;
    private bool _leftWingDamageEnabled;
    private bool _rightWingDamageEnabled;
    [SerializeField]
    private Color _enemyLaserColor;
    [SerializeField]
    private GameObjectPooler _gameObjectPool;

    private void Start() {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _enemyBoxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAudioSource = GetComponent<AudioSource>();
        _gameObjectPool = GameObject.Find("Spawn_Manager").GetComponent<GameObjectPooler>();

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
        _enemyAudioSource.clip = _explosionAudioClip;
    }
    private void OnEnable() {
        _rightWingDamageEnabled = false;
        _leftWingDamageEnabled = false;
        _pointValue = Random.Range(5,15);
        _lives = _startingLives;
        _speed = _startingSpeed;
        if (_enemyBoxCollider2D != null){
            _enemyBoxCollider2D.enabled = true;
        }
        if (_anim != null){
            _anim.SetTrigger(enableAnimationTriggerHash);
        }
        enemyIsAlive = true;
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
    // Update is called once per frame
    void Update()
    {
        
        if (enemyIsAlive){
            //MoveEnemy();
            if (Time.time > _laserNextFire){
                //Shoot();
            }
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
        GameObject enemyLaser = GameObjectPooler.Instance.Get("Enemy_Laser");
        enemyLaser.transform.position = laserPosition;
        enemyLaser.transform.localEulerAngles = Quaternion.identity.eulerAngles;
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].IsEnemyLaser = true;
            //lasers[i].IsTargetingLaser = true;
            //lasers[i].TargetPosition = _player.transform.position;
            lasers[i].GetComponent<SpriteRenderer>().color = _enemyLaserColor;
        }
        enemyLaser.SetActive(true);
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
            Laser laser = other.gameObject.GetComponent<Laser>();
            if (laser != null && laser.IsEnemyLaser == false){
                TakeDamage();
                laser.ReturnLaserToPool();
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

    private void TakeDamage(){
        _lives--;
        if (_lives < 1){
            enemyIsAlive = false;
            _player.addToScore(_pointValue);
            ExplodeEnemy();
        }else{    
            EnableDamageIndicator();
        }
    }

    private void ExplodeEnemy(){
        _anim.SetTrigger(deathAnimationTriggerHash);
        _enemyBoxCollider2D.enabled = false; // don't give more points after dying
        DisableDamageIndicators();
        StartCoroutine(RapidlyDecelerate());
        _enemyAudioSource.Play();
        StartCoroutine(DieSlowly());
    }

    IEnumerator RapidlyDecelerate(){
        while (_speed > 0.0f){
            _speed -= 0.35f;
            yield return new WaitForSeconds(0.2f);
        }
        _speed = 0f;
    }

    IEnumerator DieSlowly(){
        yield return new WaitForSeconds(2.8f);
        _player._kills++;
        GetComponent<IGameObjectPooled>().Pool.ReturnToPool(this.gameObject,"Enemy");
    }
}