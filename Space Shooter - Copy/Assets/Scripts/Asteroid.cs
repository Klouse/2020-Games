using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 6.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private float _scaleBoundHigh = 0.45f;
    private float _scaleBoundLow = 0.1f;
    private float _speed = 1.0f;
    private float _lowerYBound = -5.56f;
    private float _upperYBound = 6.99f;
    private Vector3 _randomRotation;

    private void Start() {
        if (Random.Range(0,2) == 0){
            _randomRotation = Vector3.forward;
        }else{
            _randomRotation = Vector3.back;
        }
        _speed = Random.Range(2.5f, 4f);
        float randomScaleModifier = Random.Range(_scaleBoundLow,_scaleBoundHigh);
        transform.localScale = new Vector3(randomScaleModifier,randomScaleModifier,0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_randomRotation * _rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.down * _speed * Time.deltaTime, Space.World);
        if (transform.position.y <= _lowerYBound){
            transform.position = new Vector3(transform.position.x, _upperYBound, 0.0f);
        }
    }

    public void BlowUp(){
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 0.25f);
    }

    /* private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Laser"){
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            this.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(other.gameObject);
            Destroy(this.gameObject,0.25f);
        }
    } */
}
