using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    private SpawnManager _spawnManager;
    private GameManager _gameManager;
    private Bullet _bullet;
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
    private int _maxLives;
    private bool _isAlive = true;
    
    private Vector3 startingPositionPerUpdate;
    private Vector3 playerMovementDirection;
    private Vector3 keyboardDirection;
    private Vector3 _touchPlayerOffset = new Vector3(0.0f, 0.2f, 0.0f);
    private Vector3 _worldClickPosition;

    // Laser
    private Vector3 _laserOffset = new Vector3(0f,0.5f,0f);
    public float fireRate = 0.15f;
    private float _nextFire = 0.0f;
    [SerializeField]
    private bool _canFire = true;
    [SerializeField]
    private WeaponSystem _weaponSystem;
    [SerializeField] private int _currentWeapon;
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
    private GameObject _shieldPrefab;
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
    public int currentWaveKills{get; set;}
    private int _totalKillsPerMatch;
    private int _bestScore;

    // Animations
    private Animator _anim;
    [SerializeField] private Animator _bodyAnimator;
    [SerializeField] private int _skinNumber = 5;

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
        _bullet = transform.GetComponentInChildren<Bullet>();

        _anim = GetComponent<Animator>();
        _playerAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        currentWaveKills = 0;
        _totalKillsPerMatch = 0;
        _maxLives = _lives;
        _bestScore = PlayerPrefs.GetInt("Best Score", 0);
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
            _uiManager.updateLivesUI(_lives, _maxLives);
        }
        if (_gameManager == null){
            Debug.LogError("Game Manager on Player is NULL");
        }
        if (_bodyAnimator == null)
        {
            Debug.LogError("Rocket Body Animator on Player is NULL");
        }
        if (_playerAudioSource == null){
            Debug.LogError("Audio Source on Player is NULL");
        }
        if (_weaponSystem == null){
            Debug.LogError("Weapon System on Player is NULL");
        }
        if (_bullet == null)
        {
            Debug.LogError("Bullet is NULL");
        }
        ChangeSkin(_skinNumber);
        _bullet.bulletOwner = Bullet.BulletOwner.Player;
    }

    // Update is called once per frame
    void Update()
    {
        if (_gameManager.GameStarted == true){
            MovePlayer();
        }
        
        if (Time.time > _nextFire && _canFire){
            Shoot();
        }
    }

    void MovePlayer()
    {
        startingPositionPerUpdate = transform.position;
        float zPosition = 10.0f;
        if (Input.touchCount > 0){
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;
            if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            touchPosition.z = zPosition;
            _worldClickPosition = _mainCamera.ScreenToWorldPoint(touchPosition);
            if (touch.phase == TouchPhase.Began){
                _touchPlayerOffset = transform.position - _worldClickPosition;
            }
            transform.position = Vector3.MoveTowards(transform.position, _worldClickPosition + _touchPlayerOffset, _speed * Time.deltaTime);

        }else if(Input.GetMouseButton(0)){
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 clickPosition = Input.mousePosition;
            clickPosition.z = zPosition;
            _worldClickPosition = _mainCamera.ScreenToWorldPoint(clickPosition);
            if (Input.GetMouseButtonDown(0)){
                _touchPlayerOffset = transform.position - _worldClickPosition;
            }
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
        //if (playerMovementDirection.x < 0){
        //    _anim.CrossFade("Player_Turn_Left_Anim",0.3f);
        //    _anim.Play("Player_Turn_Left_Anim");
        //}
        //if (playerMovementDirection.x > 0){
        //    _anim.CrossFade("Player_Turn_Right_Anim",0.3f);
        //    _anim.Play("Player_Turn_Right_Anim");
        //}
        //if(playerMovementDirection.x == 0){
        //    _anim.CrossFade("Idle",0.3f);
        //    _anim.Play("Idle");
        //}
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
        GameObject laserPrefab = null;
        Vector3 laserPos = transform.position;
        if (_isTripleShotActive){
            laserPrefab = GameObjectPooler.Instance.Get("Triple_Shot_Laser");
        }else if (_isFocusShotActive){
            laserPrefab = GameObjectPooler.Instance.Get("Focus_Shot_Laser");
        }else{
            laserPrefab = GameObjectPooler.Instance.Get("Default_Laser");
            laserPos += _laserOffset;
        }
        laserPrefab.transform.position = laserPos;
        laserPrefab.transform.localEulerAngles = Quaternion.identity.eulerAngles;
        laserPrefab.SetActive(true);

        //Play Laser Audio Clip
        _playerAudioSource.clip = _laserAudioClip;
        _playerAudioSource.Play();
    }

    public void ChangeWeapon(){
        // cycle weapons
        if (_currentWeapon + 1 > _weaponSystem.weaponConfigs.Count){
            _currentWeapon = 0;
        }else{
            _currentWeapon++;
        }
        _weaponSystem.EquipWeaponConfiguration(_currentWeapon);
    }

    public void ChangeWeapon(int weaponId)
    {
        switch (weaponId)
        {
            case 0:
                TripleShotActive(weaponId);
                break;
            case 1:
                SpeedActive(weaponId);
                break;
            case 2:
                ShieldActive(weaponId);
                break;
            case 3:
                FocusShotActive(weaponId);
                break;
            default:
                Debug.Log("Default Value for Power Up");
                break;
        }
    }

    public void CycleSkin()
    {
        if (_skinNumber == 31)
        {
            _skinNumber = 0;
        }
        else
        {
            _skinNumber++;
        }
        ChangeSkin(_skinNumber);
    }

    public void ChangeSkin(int skin)
    {
        switch (skin)
        {
            case 0:
                ChooseAnimation("blue_flame", "rocket_blue_red", _bodyAnimator);
                break;
            case 1:
                ChooseAnimation("blue_flame", "rocket_orange_red", _bodyAnimator);
                break;
            case 2:
                ChooseAnimation("blue_flame", "rocket_green_red", _bodyAnimator);
                break;
            case 3:
                ChooseAnimation("blue_flame", "rocket_dark_blue", _bodyAnimator);
                break;
            case 4:
                ChooseAnimation("blue_flame", "rocket_purple", _bodyAnimator);
                break;
            case 5:
                ChooseAnimation("blue_flame", "rocket_purple_white", _bodyAnimator);
                break;
            case 6:
                ChooseAnimation("blue_flame", "rocket_yellow_blue", _bodyAnimator);
                break;
            case 7:
                ChooseAnimation("blue_flame", "rocket_yellow_stars", _bodyAnimator);
                break;
            case 8:
                ChooseAnimation("yellow_flame", "rocket_blue_red", _bodyAnimator);
                break;
            case 9:
                ChooseAnimation("yellow_flame", "rocket_orange_red", _bodyAnimator);
                break;
            case 10:
                ChooseAnimation("yellow_flame", "rocket_green_red", _bodyAnimator);
                break;
            case 11:
                ChooseAnimation("yellow_flame", "rocket_dark_blue", _bodyAnimator);
                break;
            case 12:
                ChooseAnimation("yellow_flame", "rocket_purple", _bodyAnimator);
                break;
            case 13:
                ChooseAnimation("yellow_flame", "rocket_purple_white", _bodyAnimator);
                break;
            case 14:
                ChooseAnimation("yellow_flame", "rocket_yellow_blue", _bodyAnimator);
                break;
            case 15:
                ChooseAnimation("yellow_flame", "rocket_yellow_stars", _bodyAnimator);
                break;
            case 16:
                ChooseAnimation("green_flame", "rocket_blue_red", _bodyAnimator);
                break;
            case 17:
                ChooseAnimation("green_flame", "rocket_orange_red", _bodyAnimator);
                break;
            case 18:
                ChooseAnimation("green_flame", "rocket_green_red", _bodyAnimator);
                break;
            case 19:
                ChooseAnimation("green_flame", "rocket_dark_blue", _bodyAnimator);
                break;
            case 20:
                ChooseAnimation("green_flame", "rocket_purple", _bodyAnimator);
                break;
            case 21:
                ChooseAnimation("green_flame", "rocket_purple_white", _bodyAnimator);
                break;
            case 22:
                ChooseAnimation("green_flame", "rocket_yellow_blue", _bodyAnimator);
                break;
            case 23:
                ChooseAnimation("green_flame", "rocket_yellow_stars", _bodyAnimator);
                break;
            case 24:
                ChooseAnimation("pink_flame", "rocket_blue_red", _bodyAnimator);
                break;
            case 25:
                ChooseAnimation("pink_flame", "rocket_orange_red", _bodyAnimator);
                break;
            case 26:
                ChooseAnimation("pink_flame", "rocket_green_red", _bodyAnimator);
                break;
            case 27:
                ChooseAnimation("pink_flame", "rocket_dark_blue", _bodyAnimator);
                break;
            case 28:
                ChooseAnimation("pink_flame", "rocket_purple", _bodyAnimator);
                break;
            case 29:
                ChooseAnimation("pink_flame", "rocket_purple_white", _bodyAnimator);
                break;
            case 30:
                ChooseAnimation("pink_flame", "rocket_yellow_blue", _bodyAnimator);
                break;
            case 31:
                ChooseAnimation("pink_flame", "rocket_yellow_stars", _bodyAnimator);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Laser" && 
            _isAlive && 
            other.gameObject.GetComponent<Bullet>().bulletOwner == Bullet.BulletOwner.Enemy){
                TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if(_isShieldActive == false){
            _lives--;
            _uiManager.updateLivesUI(_lives, _maxLives);
            EnableDamageIndicator();
        }

        if (_lives < 1){
            ExplodePlayer();
        }
    }

    private void ExplodePlayer(){
        // Game Over
        _isAlive = false;
        CheckForBestScore();
        _spawnManager.OnPlayerDeath();
        Instantiate(_explosionPrefab, transform.position,Quaternion.identity);
        _uiManager.onGameOver();
        Destroy(this.gameObject);
    }

    public void EndOfWave(){
        _totalKillsPerMatch += currentWaveKills;
        currentWaveKills = 0;
    }
    private void CheckForBestScore(){
        int currentBestScore = PlayerPrefs.GetInt("Best Score", 0);
        int currentHighKills = PlayerPrefs.GetInt("Most Kills", 0);
        if (currentBestScore < _score){
            PlayerPrefs.SetInt("Best Score", _score);
            _uiManager.updateBestScoreUI(_score);
        }
        if (currentHighKills < _totalKillsPerMatch){
            PlayerPrefs.SetInt("Most Kills", 0);
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
        if (powerID == 2){
            _uiManager.StartShieldDisplay(cooldown);
        }
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

    public void ChooseAnimation(string animationName1, string animationName2, Animator animator, bool enable = true)
    {
        Debug.Log("Choosing animation");
        if (animator.parameterCount > 0)
        {
            Debug.Log("parameters found");
            try
            {
                Debug.Log("Setting all animations to false");
                // turn off all the non requested animation bools
                for (int anim = 0; anim < animator.parameterCount; anim++)
                {
                    Debug.Log($"Checking parameter: {animator.GetParameter(anim).name}");
                    if (animator.GetBool(animator.GetParameter(anim).name) == enable)
                    {
                        Debug.Log($"Animation setting to false");
                        animator.SetBool(animator.GetParameter(anim).name, false);
                    }
                }
                // set the requested animation to the desired enabled state
                if (animator.GetBool(animationName1) != enable)
                {
                    Debug.Log($"Setting bool {animationName1}");
                    animator.SetBool(animationName1, enable);
                    Debug.Log("Bool set successful");
                }

                // set the requested animation to the desired enabled state
                if (animator.GetBool(animationName2) != enable)
                {
                    Debug.Log($"Setting bool {animationName2}");
                    animator.SetBool(animationName2, enable);
                    Debug.Log("Bool set successful");
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
