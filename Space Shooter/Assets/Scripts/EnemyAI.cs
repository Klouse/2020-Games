using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Seeker))]
[RequireComponent (typeof(Enemy))]

public class EnemyAI : MonoBehaviour
{

    // AI
    private Transform _target;
    public float updateRate = 2f;
    private Seeker _seeker;
    private Enemy _enemy;
    // The AI's speed per second
    public float aiSpeed = 300f;
    private Rigidbody2D _rb;
    public Path path;
    public ForceMode2D fMode;
    [HideInInspector]
    public bool pathIsEnded = false;
    // waypoint enemy is moving towards
    private int currentWaypoint = 0;
    // max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();

        if (_target == null){
            Debug.Log("No target looking...");
            FindTarget();
        }
        
    }

    private void OnEnable() {
        if (_target == null){
            Debug.Log("No target looking...");
            FindTarget();
        }
        _seeker.StartPath(transform.position, _target.position, OnPathComplete);
        StartCoroutine(UpdatePath());
    }
    private void OnDisable() {
        StopAllCoroutines();
    }

    private void FindTarget(){
        _target = GameObject.Find("Player").transform;
        Debug.Log("Target: " + _target.name);
    }

    IEnumerator UpdatePath (){
        if (_target == null){
            Debug.Log("No target looking...");
            FindTarget();
        }else{
            _seeker.StartPath(transform.position, _target.position, OnPathComplete);

            yield return new WaitForSeconds(1f/updateRate );
            StartCoroutine(UpdatePath());
        }
        yield return true;
    }

    public void OnPathComplete (Path p){
        Debug.Log("Path error? " + p.error);
        if (!path.error){
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate() {
        if (_enemy.enemyIsAlive){
            if (_target == null){
                //TODO: Insert a target search here
                return;
            }

            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count){
                if (pathIsEnded)
                    return;

                Debug.Log("Path has Ended.");
                pathIsEnded = true;
                return;
            }
            pathIsEnded = false;

            Vector3 direction = ( path.vectorPath[currentWaypoint] - transform.position ).normalized;
            direction = direction * aiSpeed * Time.fixedDeltaTime;
            _rb.AddForce(direction, fMode);

            float distanceToCurrentWaypoint = Vector3.Distance (transform.position, path.vectorPath[currentWaypoint]);

            if (distanceToCurrentWaypoint < nextWaypointDistance){
                currentWaypoint++;
                return;
            }
        }
    }

}
