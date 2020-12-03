using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Seeker))]
[RequireComponent (typeof(Enemy))]

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    // AI
    private Transform _target;
    public float updateRate = 2f;
    private Seeker _seeker;
    private Enemy _enemy;
    // The AI's speed per second
    public float aiSpeed = 3.5f;
    public Path path;
    public ForceMode2D fMode;
    [HideInInspector]
    public bool pathIsEnded = false;
    // waypoint enemy is moving towards
    private int currentWaypoint = 0;
    // max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    private void OnEnable() {
        _seeker = GetComponent<Seeker>();
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_seeker == null){
            Debug.LogError("Seeker is null. PANIC!!");
        }
    }
    private void OnDisable() {
        StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        _enemy = GetComponent<Enemy>();
        if (_gameManager == null){
            Debug.LogError("Game Manager on Enemy UI is Null");
        }
    }

    public void StartPathing (Transform target){
            _target = target;
            _seeker.StartPath(transform.position, _target.position, OnPathComplete);
            StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath (){
        if (_target == null){
            Debug.Log("No target provided...PANIC!!!!");
        }else{
            _seeker.StartPath(transform.position, _target.position, OnPathComplete);
            yield return new WaitForSeconds(1f/updateRate );
            StartCoroutine(UpdatePath());
        }
        yield return true;
    }

    public void OnPathComplete (Path p){
        if (p.error){
            Debug.LogError("Path error: " + p.error);
        }else{
            path = p;
            currentWaypoint = 0;
        }
    }

    private void Update() {
        if (_enemy.enemyIsAlive){
            if (path == null)
                return;
            
            pathIsEnded = false;
            float speedFactor = 1f;
            if (currentWaypoint >= path.vectorPath.Count){
                if (pathIsEnded){
                    speedFactor = 1f;
                    return;
                }

                Debug.Log("Path has Ended.");
                pathIsEnded = true;
                return;
            }
            float distanceToCurrentWaypoint = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);
            //speedFactor = Mathf.Sqrt(distanceToCurrentWaypoint/nextWaypointDistance);

            Vector3 velocity = ( path.vectorPath[currentWaypoint] - transform.position ).normalized;
            velocity = velocity * aiSpeed * speedFactor;
            transform.position += velocity * Time.deltaTime;

            

            if (distanceToCurrentWaypoint < nextWaypointDistance){
                currentWaypoint++;
                return;
            }
        }
    }
}
