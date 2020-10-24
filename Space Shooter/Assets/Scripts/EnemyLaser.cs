using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        GameObject go;
        if (transform.position.y <= -5.5f){
            if (transform.parent != null){
                go = transform.parent.gameObject;
            }else{
                go = this.gameObject;
            }
            Destroy(go);
        }
    }
}
