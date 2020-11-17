using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private bool _movingRight = true;
    private bool _moving = true;
    [SerializeField]
    private float _secondsPaused = 3;
    [SerializeField]
    private Transform _pointA;
    [SerializeField]
    private Transform _pointB;
    [SerializeField]
    private float _speed = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_movingRight && _moving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _pointB.position, _speed * Time.deltaTime);
        }else if (_movingRight == false && _moving){
            transform.position = Vector3.MoveTowards(transform.position, _pointA.position, _speed * Time.deltaTime);
        }

        if (transform.position == _pointB.position){
            StartCoroutine(PauseThenMove(_secondsPaused,false));
        }else if (transform.position == _pointA.position){
            StartCoroutine(PauseThenMove(_secondsPaused, true));
        }
    }

    IEnumerator PauseThenMove(float seconds, bool moveRight){
        _moving = false;
        yield return new WaitForSeconds(seconds);
        _movingRight = moveRight;
        _moving = true;
    }

    private void OnTriggerEnter(Collider other){
        if (other.tag == "Player"){
            other.transform.parent = this.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player"){
            other.transform.parent = null;
        }
    }
}
