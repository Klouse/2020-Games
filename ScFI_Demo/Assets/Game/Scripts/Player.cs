using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    float _gravity = 9.81f;
    [SerializeField]
    float _speed = 3.5f;
    NavMeshAgent _controller;
    [SerializeField]
    GameObject _muzzleFlash;
    ParticleSystem _muzzleParticleSys;
    [SerializeField]
    ObjectPooler _gameObjectPool;
    [SerializeField]
    AudioClip _muzzleAudioClip;
    [SerializeField]
    AudioClip _coinAudioClip;
    [SerializeField]
    private AudioClip _purchaseAudioClip;
    [SerializeField]
    AudioManager _audioManager;

    [SerializeField]
    UIManager _uiManager;
    int _currentAmmo;
    [SerializeField]
    int _maxAmmo = 50;
    private bool _isReloading = false;
    private bool _hasGun;
    [SerializeField]
    private GameObject _weapon;
    public bool _hasCoin {get; private set;}

    // Start is called before the first frame update
    void Start()
    {
        if (_uiManager == null){
            Debug.LogError("UIManager on Player is NULL");
        }
        if (_weapon == null){
            Debug.LogError("Weapon is NULL");
        }
        _hasCoin = false;
        _hasGun = false;
        _weapon.SetActive(false);
        _currentAmmo = _maxAmmo;
        _uiManager.CurrentAmmo = _currentAmmo;
        _controller = GetComponent<NavMeshAgent>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _muzzleParticleSys = _muzzleFlash.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && (_currentAmmo > 0) && _isReloading == false && _hasGun){
            Shoot();
        }else{
            StopShooting();
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetKeyDown(KeyCode.R) && _isReloading == false){
            StartCoroutine(Reload());
        }

        Movement();
    }

    void Shoot(){
        _currentAmmo--;
        _uiManager.CurrentAmmo = _currentAmmo;
        if (_muzzleParticleSys.isPlaying == false){
            _muzzleParticleSys.Play();
            _audioManager.PlayBulletAudioClip(_muzzleAudioClip, true);
        }

        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hitInfo;
        if (Physics.Raycast(rayOrigin, out hitInfo, Mathf.Infinity)){
            var hitMarker = _gameObjectPool.Get();
            hitMarker.transform.position = hitInfo.point;
            hitMarker.transform.localEulerAngles = Quaternion.LookRotation(hitInfo.normal).eulerAngles;
            hitMarker.SetActive(true);
            // destroy crate if it is one
            Destructable crate = hitInfo.transform.GetComponent<Destructable>();
            if (crate != null){
                crate.DestroyCrate();
            }
        }
    }

    void StopShooting(){
        _muzzleParticleSys.Stop();
        _audioManager.StopBulletAudio();
    }

    IEnumerator Reload(){
        _isReloading = true;
        yield return new WaitForSeconds(1.5f);
        _currentAmmo = _maxAmmo;
        _uiManager.CurrentAmmo = _currentAmmo;
        _isReloading = false;
    }

    void Movement(){
        Vector3 velocity;
        float newSpeed;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (horizontalInput > 0 || horizontalInput < 0 || verticalInput > 0 || verticalInput < 0){
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);
        if (Input.GetKey(KeyCode.LeftShift)){
            newSpeed = _speed * 2;
        }else{
            newSpeed = _speed;
        }
        velocity = direction * newSpeed;
        velocity.y -= _gravity;
        velocity = transform.transform.TransformDirection(velocity);
        _controller.Move(velocity * Time.deltaTime);
    }

    public void PickUpCoin(){
        _hasCoin = true;
        _audioManager.PlaySFXAudioClip(_coinAudioClip);
        _uiManager.EnableCoin();
    }

    public void PayForGun()
    {
        if (_hasCoin){
            _hasCoin = false;
            _audioManager.PlaySFXAudioClip(_purchaseAudioClip);
            _uiManager.DisableCoin();
            _uiManager.EnableAmmo();
            _hasGun = true;
            _weapon.SetActive(true);
        }
    }
}
