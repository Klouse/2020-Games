using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // speed variable
    [SerializeField]
    float _playerLaserSpeed = 15f;
    [SerializeField]
    float _enemyLaserSpeed = 5f;

    public bool IsEnemyLaser {get; set;} = false;

    // Update is called once per frame
    void Update()
    {
        GameObject go;
        if (IsEnemyLaser){
            Move(Vector3.down, _enemyLaserSpeed);
        }else{
            Move(Vector3.up, _playerLaserSpeed);
        }

        if (transform.position.y >= 12.0f || transform.position.y <= -6.0f){
            if (transform.parent != null){
                go = transform.parent.gameObject;
            }else{
                go = gameObject;
            }
            Destroy(go);
        }
    }

    void Move(Vector3 direction, float speed){
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && IsEnemyLaser){
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null){
                player.TakeDamage();
                Destroy(this.gameObject);
            }
        }
    }
}
