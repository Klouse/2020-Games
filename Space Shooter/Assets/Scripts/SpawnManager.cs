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
        public int numberOfEnemies;
        public int secondsToWaitBeforeSpawning;
        public float rateBetweenEnemies;
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
            Debug.Log("Wave countdown: " + _waveCountdown);
            if (_waveCountdown <= 0){
                if (state != SpawnState.SPAWNING){
                    StartCoroutine(SpawnEnemyWaveRoutine(_waves[_nextWave]));
                }
            }else{
                _waveCountdown -= Time.deltaTime;
            }
        }
    }

    private void WaveCompleted(){
        Debug.Log("Wave Completed -- Good Job");
        state = SpawnState.COUNTING;
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
        var enemyCount = _waves[_nextWave].numberOfEnemies;
        if (searchCountdown <= 0){
            searchCountdown = 1f;
            if (_player._kills >= enemyCount){
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
 
    IEnumerator SpawnEnemyWaveRoutine(Wave wave)
    {
        Debug.Log("Spawning Wave: "+ wave.name);
        state = SpawnState.SPAWNING;
        for (int i = 0; i < wave.numberOfEnemies; i++)
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
        newEnemy.transform.localEulerAngles = currentSpawnPoint.localEulerAngles;
        newEnemy.transform.parent = _enemyContainer.transform;
        newEnemy.SetActive(true);
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
