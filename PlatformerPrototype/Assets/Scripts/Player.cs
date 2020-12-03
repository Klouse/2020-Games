using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField]
    private UIManager _uiManager;
    
    private float _horizontalInput;
    private float _yVelocity;
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private float _gravity = 1.0f;
    [SerializeField]
    private float _jumpHeight = 15.0f;
    private bool _canDoubleJump = false;
    // player coins int
    private int _playerCoins = 0;
    [SerializeField]
    private int _lives = 3;
    private bool _spacePressed = false;
    private Vector3 _respawnPosition;

    // Start is called before the first frame update
    void Start()
    {
        _respawnPosition = new Vector3(-21f, 20f, 0f);
        _controller = GetComponent<CharacterController>();
        if (_uiManager == null){
            Debug.LogError("UI Manager on Player is Null");
        }
        awardCoins(0);
        updateLives(_lives);
    }

    private void Update() {
        if (_lives > 0){
            HandleInput();
            if (transform.position.y < -11f){
                TakeDamage();
            }
        }
    }

    private void TakeDamage(){
        Vector3 movePosition = _respawnPosition - transform.position;
        _controller.Move(movePosition);
        _lives--;
        updateLives(_lives);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            _spacePressed = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_lives > 0){
            _horizontalInput = Input.GetAxis("Horizontal");
        }else{
            _horizontalInput = 0f;
        }
        Vector3 direction = new Vector3(_horizontalInput, 0, 0);
        Vector3 velocity = direction * _speed;
        if (_controller.isGrounded){
            if (_spacePressed){
                _yVelocity = _jumpHeight;
                _canDoubleJump = true;
            }
        }else{
            if (_spacePressed && _canDoubleJump){
                _yVelocity = _jumpHeight;
                _canDoubleJump = false;
            }
            _yVelocity -= _gravity;
        }
        _spacePressed = false;
        velocity.y = _yVelocity;
        _controller.Move(velocity * Time.deltaTime);
        }
    public void awardCoins(int addedCoins){
        _playerCoins += addedCoins;
        _uiManager.UpdateCoinUI(_playerCoins);
    }

    private void updateLives(int lives)
    {
        _uiManager.UpdateLivesUI(lives);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Platform"){
            _yVelocity = 0f;
            _canDoubleJump = false;
        }
    }
    
}
