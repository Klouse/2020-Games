using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int preWarmSize;
    }
    public List<Pool> _pools;
    private Dictionary<string, Queue<GameObject>> _poolDictionary;
    public static GameObjectPooler Instance { get; private set;}

    private void Awake(){
        Instance = this;
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (var pool in _pools)
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            _poolDictionary.Add(pool.tag, newQueue);
            if (pool.preWarmSize > 0){
                AddObject(pool.preWarmSize, pool.tag, pool.prefab);
            }
        }
    }
    
    public GameObject Get(string poolTag){
        // return object from pool
        if (_poolDictionary[poolTag].Count == 0){
            GameObject prefab = null;
            foreach (var pool in _pools)
            {
                if (pool.tag == poolTag)
                {
                    prefab = pool.prefab;
                }
            }
            // add new object
            if (prefab != null)
                AddObject(1, poolTag, prefab);
        }
        var objToReturn =  _poolDictionary[poolTag].Dequeue();
        
        Debug.Log("Obj Name: "+ objToReturn.name);
        return objToReturn;
    }

    void AddObject(int count, string poolTag, GameObject prefab){
        for (int i = 0; i < count; i++)
        {
            var newObject = GameObject.Instantiate(prefab);
            IGameObjectPooled newObjectPoolAgent = newObject.GetComponent<IGameObjectPooled>();
            newObjectPoolAgent.Pool = this;
            newObjectPoolAgent.poolTag = poolTag;
            newObject.SetActive(false);
            Debug.Log("New Object Added " + newObject.name);
            _poolDictionary[poolTag].Enqueue(newObject);
        }
    }

    public void ReturnToPool(GameObject objectToAdd, string poolTag){
        objectToAdd.SetActive(false);
        _poolDictionary[poolTag].Enqueue(objectToAdd);
    }
}
