using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private Camera _mainCamera;
    private UIManager _uiManager;

    // Player and bounds
    [SerializeField]
    private float _speed = 10.0f;
    private float _speedPowerMultiplier = 2.0f;
     private float _lowerYBound = -4.2f;
    private float _upperYBound = 6.25f;
    private float _lowerXBound = -2.72f;
    private float _upperXBound = 2.72f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _startingShieldLives = 3;
    private int _activeShieldLives = 0;
    private bool _isAlive = true;
    
    private Vector3 startingPositionPerUpdate;
    private Vector3 playerMovementDirection;
    private Vector3 keyboardDirection;
    private Vector3 _touchPlayerOffset = new Vector3(0.0f, 0.2f, 0.0f);
    private Vector3 _worldClickPosition;

    // Laser
    [SerializeField]
    private GameObject _defaultLaserPrefab;
    private Vector3 _laserOffset = new Vector3(0f,0.5f,0f);
    public float fireRate = 0.15f;
    private float _nextFire = 0.0f;

    // Power Ups
    [SerializeField]
    private bool _isTripleShotActive;
    [SerializeField]
    private bool _isFocusShotActive;
    [SerializeField]
    private bool _isSpeedActive;
    [SerializeField]
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldPrefab;
    [SerializeField]
    private GameObject _focusShotPrefab;
    [SerializeField]
    private float _shieldDuration = 10.0f;
    [SerializeField]
    private GameObject _shieldVisual;
    [SerializeField]
    private GameObject _leftWingDamageIndicatorVisual;
    private bool _leftWingDamageEnabled = false;
    [SerializeField]
    private GameObject _rightWingDamageIndicatorVisual;
    private bool _rightWingDamageEnabled = false;

    // Score
    private int _score;
    private int _bestScore;

    // Animations
    private Animator _anim;
    //Sound
    [SerializeField]
    private AudioClip _laserAudioClip;
    private AudioSource _playerAudioSource;
    [SerializeField]
    private GameObject _explosionPrefab;

    private void Awake() {
        _mainCamera = Camera.main;
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _anim = GetComponent<Animator>();
        _playerAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        _bestScore = PlayerPrefs.GetInt("Best Score", 0);
        _lives = 3;
        transform.position = new Vector3(0f,-1.0f,0f);
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager on Player is NULL");
        }
        if (_uiManager == null){
            Debug.LogError("UI Manager on Player is NULL");
        }else {
            _uiManager.updateScoreUI(_score);
            _uiManager.updateBestScoreUI(_bestScore);
            _uiManager.updateLivesUI(_lives);
        }
        if (_gameManager == null){
            Debug.LogError("Game Manager on Player is NULL");
        }
        if (_anim == null){
            Debug.LogError("Animator on Player is NULL");
        }
        if (_playerAudioSource == null){
            Debug.LogError("Audio Source on Player is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.GameStarted == true){
            MovePlayer();
        }
        
        if (Time.time > _nextFire){
            Shoot();
        }
    }

    void MovePlayer()
    {
        startingPositionPerUpdate = transform.position;
        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            touchPosition.z = 10f;
            _worldClickPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
            transform.position = Vector3.MoveTowards(transform.position, _worldClickPosition + _touchPlayerOffset, _speed * Time.deltaTime);
        }else if(Input.GetMouseButton(0)){
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 clickPosition = Input.mousePosition;
            clickPosition.z = 10.0f;
            _worldClickPosition = _mainCamera.ScreenToWorldPoint(clickPosition);
            transform.position = Vector3.MoveTowards(transform.position,_worldClickPosition + _touchPlayerOffset,_speed * Time.deltaTime);
        }else{
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            keyboardDirection = new Vector3(horizontalInput, verticalInput, 0);
            transform.Translate(keyboardDirection * (_speed) * Time.deltaTime);
        }
        playerMovementDirection = transform.position - startingPositionPerUpdate;
        BindPlayer();
        AnimatePlayer();
    }

    void AnimatePlayer(){
        // Handle the player animation state change based on player movement
        if (playerMovementDirection.x < 0){
            _anim.CrossFade("Player_Turn_Left_Anim",0.3f);
            _anim.Play("Player_Turn_Left_Anim");
        }
        if (playerMovementDirection.x > 0){
            _anim.CrossFade("Player_Turn_Right_Anim",0.3f);
            _anim.Play("Player_Turn_Right_Anim");
        }
        if(playerMovementDirection.x == 0){
            _anim.CrossFade("Idle",0.3f);
            _anim.Play("Idle");
        }
    }

    void BindPlayer(){
        // Bind the player to the playable portion of the screen
        if (transform.position.x >= _upperXBound){
            transform.position = new Vector3(_upperXBound, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= _lowerXBound){
            transform.position = new Vector3(_lowerXBound, transform.position.y, transform.position.z);
        }
        if (transform.position.y >= _upperYBound){
            transform.position = new Vector3(transform.position.x, _upperYBound, transform.position.z);
        }
        else if (transform.position.y <= _lowerYBound){
            transform.position = new Vector3(transform.position.x, _lowerYBound, transform.position.z);
        }
    }

    void Shoot()
    {
        _nextFire = Time.time + (fireRate/100);
        GameObject laserPrefab;
        Vector3 laserPos = transform.position;
        if (_isTripleShotActive){
            laserPrefab = _tripleShotPrefab;
        }else if (_isFocusShotActive){
            laserPrefab = _focusShotPrefab;
        }else{
            laserPrefab = _defaultLaserPrefab;
            laserPos += _laserOffset;
        }
        Instantiate(laserPrefab,laserPos,Quaternion.identity);

        //Play Laser Audio Clip
        _playerAudioSource.clip = _laserAudioClip;
        _playerAudioSource.Play();
    }

    public void TakeDamage()
    {
        if(_isShieldActive){
            _activeShieldLives--;
            if (_activeShieldLives <= 0){
                _shieldVisual.SetActive(false);
                _isShieldActive = false;
            }
        }else{
            _lives--;
            _uiManager.updateLivesUI(_lives);
            EnableDamageIndicator();
        }

        if (_lives < 1){
            ExplodePlayer();
        }
    }

    private void ExplodePlayer(){
        // Game Over
        _isAlive = false;
        _gameManager.GameStarted = false;
        CheckForBestScore();
        _spawnManager.OnPlayerDeath();
        Instantiate(_explosionPrefab, transform.position,Quaternion.identity);
        _uiManager.onGameOver();
        Destroy(this.gameObject);
    }
    private void CheckForBestScore(){
        int currentBestScore = PlayerPrefs.GetInt("Best Score", 0);
        if (currentBestScore < _score){
            PlayerPrefs.SetInt("Best Score", _score);
            _uiManager.updateBestScoreUI(_score);
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

    public void TripleShotActive(int id){
        _isTripleShotActive = true;
        StartCoroutine(PowerDownRoutine(3.0f,id));
    }

    public void FocusShotActive(int id){
        _isFocusShotActive = true;
        StartCoroutine(PowerDownRoutine(5.0f,id));
    }

    public void SpeedActive(int id){
        _isSpeedActive = true;
        _speed *= _speedPowerMultiplier;
        StartCoroutine(PowerDownRoutine(5.0f,id));
    }

    public void ShieldActive(int id){
        if (_isShieldActive == false){
            _activeShieldLives = _startingShieldLives;
            _isShieldActive = true;
            _shieldVisual.SetActive(true);
            StartCoroutine(PowerDownRoutine(_shieldDuration,id));
        }
    }

    public void addToScore(int val){
        // add to the score that exists, could be positive or negative, so bind to 0 if it hits it.
        _score += val;
        _uiManager.updateScoreUI(_score);
    }

    IEnumerator PowerDownRoutine(float cooldown, int powerID){
        yield return new WaitForSeconds(cooldown);
        switch (powerID){
            case 0:
                _isTripleShotActive = false;
                break;
            case 1:
                _speed /= _speedPowerMultiplier;
                _isSpeedActive = false;
                break;
            case 2:
                _shieldVisual.SetActive(false);
                _isShieldActive = false;
                break;
            case 3:
                _isFocusShotActive = false;
                break;
            default:
                Debug.Log("Default Value Power Down Routine");
                break;
        }
    }
}
