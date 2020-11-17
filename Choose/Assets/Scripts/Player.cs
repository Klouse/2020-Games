using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _characterController;
    [SerializeField]
    private float _startingSpeed = 0.1f;
    [SerializeField]
    private float _currentSpeed;
    private Coroutine _activeCoroutine;
    private float _tickSpeed;
    [SerializeField]
    private GameObject _carryBox;
    private Vector3 _scalingVector = new Vector3(0.1f, 0.0f, 0.1f);
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null)
            Debug.LogError("Character Controller is NULL");

        if (_carryBox == null)
            Debug.LogError("Carry Box is NULL");

        _currentSpeed = _startingSpeed;
        _tickSpeed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput);

        if (true){
            _characterController.Move(direction * _currentSpeed);
            if (horizontalInput > 0f || horizontalInput < 0f || verticalInput > 0f || verticalInput < 0f){
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void StartGainingStats(int stat){
        Debug.Log("Stat: "+ stat);
        switch (stat)
        {
            case 1:
                _activeCoroutine = StartCoroutine(GainSpeed());
                break;
            case 2:
                _activeCoroutine = StartCoroutine(GainWidth());
                break;
            default:
                Debug.Log("No");
                break;
        }
        _activeCoroutine = StartCoroutine(GainSpeed());
    }

    public void StopGainingStats(){
        // stop currently running coroutine
        if (_activeCoroutine != null){
            StopCoroutine(_activeCoroutine);
            _activeCoroutine = null;
        }
    }

    IEnumerator GainSpeed(){
        while(true){
            yield return new WaitForSeconds(_tickSpeed);
            Debug.Log("Ticking Speed");
            if (_currentSpeed < 0.1f){
                _currentSpeed += 0.01f;
            }
        }
    }
    
    IEnumerator GainWidth(){
        while(true){
            yield return new WaitForSeconds(_tickSpeed);
            Debug.Log("Ticking Scale");
            _carryBox.transform.localScale += _scalingVector;
        }
    }
}
