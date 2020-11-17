using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookY : MonoBehaviour
{
    [SerializeField]
    float _sensitivity = 1.0f;

    // Update is called once per frame
    void Update()
    {
        float _mouseY = -Input.GetAxis("Mouse Y");
        Vector3 lookRotation = transform.localEulerAngles;
        lookRotation.x += _mouseY * _sensitivity;

        transform.localEulerAngles = lookRotation;
        
    }
}
