using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public float upForce = 1.0f;
    public float sideForce = 0.1f;
    private Rigidbody _myRigidbody;

    private void Awake() {
        _myRigidbody = GetComponent<Rigidbody>();
    }
    void OnEnable()
    {
        // apply a random force
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce/2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);

        Vector3 force = new Vector3(xForce, yForce, zForce);

        _myRigidbody.velocity = force;
    }

    private void OnDisable() {
        _myRigidbody.velocity = new Vector3(0,0,0);
    }
}
