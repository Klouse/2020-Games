using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum SpawnState{SPAWNING, WAITING, COUNTING}
    private SpawnState state = SpawnState.COUNTING;

    [System.Serializable]
    public class Wave{
        public string name;
        public int secondsToWaitBeforeSpawning;
        public float rateBetweenEnemies;
        public int[] endPointsForEnemies;
    }

    public Wave[] _waves;
    private int _nextWave = 0;
    private float _waveCountdown;

    [SerializeField]
    int _positionSwitch = 5;
    [SerializeField]
    private GameObject[] _powerUpPrefabs;
    [SerializeField]
    private GameObject _asteroidPrefab;
    private float _upperYBound = 7.0f;
    [SerializeField]
    private Transform[] _spawnPoints;
    [SerializeField]
    private float _firstEndPointY;
    [SerializeField]
    private GameObject _endPointPrefab;
    [SerializeField]
    private GameObject _endPointContainer;
    private List<Transform> _allEndPoints;
    [SerializeField]
    private float maxColumns = 7;
    [SerializeField]
    private float _endPointGridOffset = 0.5f;
    private Queue<Transform> _currentWaveEndPoints;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private GameObject _asteroidContainer;
    private bool _gameplaySpawning = false;
    private int _asteroidLimit = 0;
    private Coroutine _asteroidCoroutine;
    private List<int> _asteroidZValues = new List<int>();
    [SerializeField]
    private float _enemySpawnRate = 2.0f;
    [SerializeField]
    private int _enemySpawnRateIncreaseInterval = 30;
    private float searchCountdown = 1f;
    private Player _player;

    private void Awake() {
        for (int i = 0; i < _asteroidLimit; i++)
        {
            _asteroidZValues.Add(i+1);
        }
    }
    
    private void Start() {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _waveCountdown = _waves[0].secondsToWaitBeforeSpawning;
        StartSpawningPreGameplay();
        CreateEndPoints(maxColumns * maxColumns);
    }

    private void CreateEndPoints(float maxEndPoints)
    {
        _allEndPoints = new List<Transform>();
        int currentColumn = 0;
        float offset = _endPointGridOffset;
        float defaultXPosition = (0f - (offset * ((maxColumns-1)/2)));
        float x = defaultXPosition;
        float y = _firstEndPointY;
        float z = 0f;
        for (int i = 0; i < maxEndPoints; i++)
        {
            GameObject newEndpoint = Instantiate(_endPointPrefab, new Vector3(x, y, z), Quaternion.identity);
            newEndpoint.name = _endPointPrefab.name + i;
            newEndpoint.transform.parent = _endPointContainer.transform;
            _allEndPoints.Add(newEndpoint.transform);
            if (currentColumn >= (maxColumns-1)){
                x = defaultXPosition;
                y -= offset;
                currentColumn = 0;
            }else{
                x += offset;
                currentColumn++;
            }
        }
    }

    private void Update() {
        if (state == SpawnState.WAITING){
            if (AnyEnemiesAlive()){
                return;
            }else{
                WaveCompleted();
            }
        }
        if (_gameplaySpawning){
            if (_waveCountdown <= 0){
                if (state != SpawnState.SPAWNING){
                    StartCoroutine(StartSpawnEnemyWaveRoutine(_waves[_nextWave]));
                }
            }else{
                _waveCountdown -= Time.deltaTime;
            }
        }
    }

    private void WaveCompleted(){
        Debug.Log("Wave Completed -- Good Job");
        state = SpawnState.COUNTING;
        _player.EndOfWave();
        if (_nextWave + 1 > _waves.Length - 1){
            // game over, now loop
            _nextWave = 0;
            Debug.Log("All Waves Complete, Looping");
        }else{
            _nextWave++;
            _waveCountdown = _waves[_nextWave].secondsToWaitBeforeSpawning;
        }
    }

    private bool AnyEnemiesAlive(){
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0){
            searchCountdown = 1f;
            if (_player.currentWaveKills >= _waves[_nextWave].endPointsForEnemies.Length){
                return false;
            }
        }
        return true;
    }

    private void StartSpawningPreGameplay()
    {
        _asteroidCoroutine = StartCoroutine(SpawnAsteroids());
    }

    public void StartSpawningGameplay(float secondsToWaitBeforeSpawning)
    {
        _gameplaySpawning = true;
        StartCoroutine(SpawnPowerUpRoutine(secondsToWaitBeforeSpawning));
    }
 
    IEnumerator StartSpawnEnemyWaveRoutine(Wave wave)
    {
        _currentWaveEndPoints = new Queue<Transform>();
        for (int i = 0; i < wave.endPointsForEnemies.Length; i++){
            _currentWaveEndPoints.Enqueue(_allEndPoints[wave.endPointsForEnemies[i]]);
        }
        Debug.Log("Spawning Wave: "+ wave.name);
        state = SpawnState.SPAWNING;
        for (int i = 0; i < wave.endPointsForEnemies.Length; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f/ wave.rateBetweenEnemies);
        }
        state = SpawnState.WAITING;
        yield break;
    }

    private void SpawnEnemy(){
        if (_spawnPoints.Length == 0){
            Debug.LogError("No Spawn Points!!");
            return;
        }
        Transform currentSpawnPoint = _spawnPoints[Random.Range(0,_spawnPoints.Length)];
        var newEnemy = GameObjectPooler.Instance.Get("Enemy");
        newEnemy.transform.position = currentSpawnPoint.position;
        //newEnemy.transform.localEulerAngles = currentSpawnPoint.localEulerAngles;
        newEnemy.transform.parent = _enemyContainer.transform;
        Enemy newEnemyScript = newEnemy.GetComponent<Enemy>();
        newEnemyScript.SetSpawnAndDestination(currentSpawnPoint,_currentWaveEndPoints.Dequeue());
        //newEnemy.GetComponent<EnemyAI>().StartPathing(_currentWaveEndPoints.Dequeue());
        newEnemy.SetActive(true);
        newEnemyScript.TweenMoveEnemy();
    }

    IEnumerator SpawnPowerUpRoutine(float secondsToWaitBeforeSpawning)
    {
        yield return new WaitForSeconds(secondsToWaitBeforeSpawning);
        while (_gameplaySpawning)
        {
            Vector3 randomSpawnPos = new Vector3(Random.Range(-2.55f,2.55f),_upperYBound, 0.0f);
            if (_powerUpPrefabs.Length > 0){
                GameObject newPowerUp = Instantiate(_powerUpPrefabs[Random.Range(0,_powerUpPrefabs.Length)],randomSpawnPos,Quaternion.identity);
                newPowerUp.transform.parent = _powerUpContainer.transform;
                yield return new WaitForSeconds(Random.Range(3.0f,7.0f));
            }
        }
    }

    IEnumerator SpawnAsteroids()
    {
        while (_asteroidContainer.transform.childCount < _asteroidLimit)
        {
            int randomSortingOrder = _asteroidZValues[Random.Range(0,_asteroidZValues.Count)];
            _asteroidZValues.Remove(randomSortingOrder);
            Vector3 randomSpawnPos = new Vector3(Random.Range(-2.85f, 2.85f),_upperYBound, 0.0f);
            GameObject newAsteroid = Instantiate(_asteroidPrefab, randomSpawnPos, Quaternion.identity);
            newAsteroid.GetComponent<SpriteRenderer>().sortingOrder = randomSortingOrder;
            newAsteroid.transform.parent = _asteroidContainer.transform;
            yield return new WaitForSeconds(Random.Range(0.5f, 2.5f));
        }
    }

    public void OnPlayerDeath()
    {
        _gameplaySpawning = false;
    }
}
