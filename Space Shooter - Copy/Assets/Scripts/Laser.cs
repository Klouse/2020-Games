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
    public bool IsTargetingLaser {get; set;} = false;
    public Vector3 TargetPosition {get; set;}

    private void OnEnable() {
        if (IsTargetingLaser){
            Vector3 targetDir = TargetPosition - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);
            Debug.Log("Angle: " + angle);
            transform.parent.transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (transform.position.y >= 12.0f || transform.position.y <= -6.0f){
            ReturnLaserToPool();
        }
    }

    public void ReturnLaserToPool(){
        GameObject go;
        if (transform.parent != null){
            go = transform.parent.gameObject;
        }else{
            go = gameObject;
        }
        IGameObjectPooled poolAgent  = go.GetComponent<IGameObjectPooled>();
        poolAgent.Pool.ReturnToPool(go, poolAgent.poolTag);
    }

    void Move(){
        Vector3 direction;
        float speed;
        if (IsEnemyLaser){
            direction = Vector3.down;
            speed = _enemyLaserSpeed;
        }else{
            direction = Vector3.up;
            speed = _playerLaserSpeed;
        }
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && IsEnemyLaser){
            Player player = other.gameObject.GetComponent<Player>();
            if (player != null){
                player.TakeDamage();
                ReturnLaserToPool();
            }
        }
    }
}
