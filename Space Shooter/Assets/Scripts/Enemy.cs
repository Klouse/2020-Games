using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _startingSpeed = 3.5f;
    private int _lives = 3;
    [SerializeField]
    private int _startingLives = 3;
    private float _lowerYBound = -5.56f;
    private float _upperYBound = 6.99f;
    private int _pointValue = 10;
    private Player _player;
    //private Animator _anim;
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
    private Vector3 _enemyResetPosition = new Vector3(-20f,0,0);
    private Transform _moveDestination;
    private Transform _currentSpawnPoint;
    private Sequence _wiggleSequence;
    private Transform _engine;

    private void Start() {
        _player = GameObject.Find("Player").GetComponent<Player>();
        //_anim = GetComponent<Animator>();
        _enemyBoxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAudioSource = GetComponent<AudioSource>();
        _gameObjectPool = GameObject.Find("Spawn_Manager").GetComponent<GameObjectPooler>();

        if (_player == null){
            Debug.LogError("Player reference on Enemy is NULL");
        }
        //if (_anim == null){
            //Debug.LogError("Animator on Enemy is NULL");
        //}
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
        //_pointValue = Random.Range(5,15);
        _lives = _startingLives;
        _speed = _startingSpeed;
        if (_enemyBoxCollider2D != null){
            _enemyBoxCollider2D.enabled = true;
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
            //TweenMoveEnemy();
            //AiMoveEnemy();
            //MoveEnemy();
            if (Time.time > _laserNextFire){
                //Shoot();
            }
        }
    }

    public void TweenMoveEnemy(){
        transform.DOMove(_moveDestination.position,_speed).SetEase(Ease.OutSine)
            .From(_currentSpawnPoint.position)
                .OnComplete(AtDestination);
    }

    private void AtDestination(){
        try{
            StartCoroutine(DestinationWiggle());

        }catch{
            Debug.Log("Enemy is not active to start wiggle");
        }
    }

    IEnumerator DestinationWiggle(){
        float wiggleTollerance = 0.25f;
        float wiggleSpeed = _speed;
        Ease wiggleEase = Ease.InSine;
        Vector3[] wigglePositions = {
        _moveDestination.position + new Vector3(wiggleTollerance, -wiggleTollerance, 0f),
        _moveDestination.position + new Vector3(0, -(wiggleTollerance*2), 0f),
        _moveDestination.position + new Vector3(-wiggleTollerance, -wiggleTollerance, 0f),
        _moveDestination.position
        };
        _wiggleSequence = DOTween.Sequence();
        for (int i = 0; i < wigglePositions.Length; i++)
        {
            _wiggleSequence.Append(transform.DOMove(wigglePositions[i], wiggleSpeed).SetEase(wiggleEase));
        }
        _wiggleSequence.SetLoops(-1);
        yield return true;
    }

    public void SetSpawnAndDestination(Transform spawnpoint, Transform endpoint){
        _currentSpawnPoint = spawnpoint;
        _moveDestination = endpoint;
    }

    private void MoveEnemy(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= _lowerYBound){
            transform.position = new Vector3(transform.position.x, _upperYBound, transform.position.z);
        }
    }

    private void AiMoveEnemy(){
    }

    IEnumerator Shoot(){
        Draw.Disc(transform.position, Quaternion.identity, 0.5f, Color.red);
        yield return new WaitForSeconds(0.1f);
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
        }
        if(_lives < (_lives/3)){
            EnableDamageIndicator();
        }
    }

    private void ExplodeEnemy(){
        _wiggleSequence.Kill();
        //_anim.SetBool("EnemyDead", true);
        _enemyBoxCollider2D.enabled = false; // don't give more points after dying
        DisableDamageIndicators();
        _enemyAudioSource.Play();
        transform.DOKill();
        //StartCoroutine(RapidlyDecelerate());
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
        _player.currentWaveKills++;
        //yield return new WaitForSeconds(2.8f);
        transform.position = _enemyResetPosition;
        //yield return new WaitForSeconds(0.5f);
        //_anim.SetBool("EnemyDead", false);
        yield return new WaitForSeconds(0.5f);
        GetComponent<IGameObjectPooled>().Pool.ReturnToPool(this.gameObject,"Enemy");
    }
}