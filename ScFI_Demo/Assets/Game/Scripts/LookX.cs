using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookX : MonoBehaviour
{
    [SerializeField]
    float _sensitivity = 1.0f;

    // Update is called once per frame
    void Update()
    {
        float _mouseX = Input.GetAxis("Mouse X");
        Vector3 lookRotation = transform.localEulerAngles;
        lookRotation.y += _mouseX * _sensitivity;

        transform.localEulerAngles = lookRotation;
    }
}
