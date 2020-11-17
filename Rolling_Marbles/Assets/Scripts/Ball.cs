using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField]
    float gravity = 9.81f;
    private CharacterController _controller;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        if (_controller == null){
            Debug.LogError("Ball Character Controller is null");
        }
    }

    private void FixedUpdate() {
        //Vector3 direction;
        //direction = 
        //_controller.Move(direction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
