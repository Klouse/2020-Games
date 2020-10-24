using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerUpPrefabs;
    [SerializeField]
    private GameObject _asteroidPrefab;
    private float _upperYBound = 7.0f;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject _powerUpContainer;
    [SerializeField]
    private GameObject _asteroidContainer;
    private bool _gameplaySpawning = true;
    private int _asteroidLimit = 15;
    private Coroutine _asteroidCoroutine;
    private List<int> _asteroidZValues = new List<int>();
    [SerializeField]
    private float _enemySpawnRate = 2.0f;
    [SerializeField]
    private int _enemySpawnRateIncreaseInterval = 30;

    private void Awake() {
        for (int i = 0; i < _asteroidLimit; i++)
        {
            _asteroidZValues.Add(i+1);
        }
    }
    

    private void Start() {
        StartSpawningPreGameplay();
    }

    private void StartSpawningPreGameplay()
    {
        _asteroidCoroutine = StartCoroutine(SpawnAsteroids());

    }

    public void StartSpawningGameplay(float secondsToWaitBeforeSpawning)
    {
        StartCoroutine(SpawnEnemyRoutine(secondsToWaitBeforeSpawning));
        StartCoroutine(IncreaseSpawnRate(_enemySpawnRateIncreaseInterval));
        StartCoroutine(SpawnPowerUpRoutine(secondsToWaitBeforeSpawning));
    }
 
    IEnumerator SpawnEnemyRoutine(float secondsToWaitBeforeSpawning)
    {
        yield return new WaitForSeconds(secondsToWaitBeforeSpawning);
        while (_gameplaySpawning)
        {
            Vector3 randomSpawnPos = new Vector3(Random.Range(-2.3f,2.3f), _upperYBound, 0.0f);
            GameObject newEnemy = Instantiate(_enemyPrefab, randomSpawnPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate);
        }
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

    IEnumerator IncreaseSpawnRate(int secondsBetweenIncrease)
    {
        while (_gameplaySpawning){
            yield return new WaitForSeconds(secondsBetweenIncrease);
            if (_enemySpawnRate >= 0.1)
                _enemySpawnRate-= 0.1f;
        }
    }

    public void OnPlayerDeath()
    {
        _gameplaySpawning = false;
    }
}
