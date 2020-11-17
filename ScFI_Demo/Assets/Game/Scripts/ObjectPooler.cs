using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject _prefab;
    public static ObjectPooler Instance {get; private set;}
    private Queue<GameObject> _objects = new Queue<GameObject>();
    private void Awake() {
        Instance = this;
    }
    private void AddObjects(int num){
        for (int i = 0; i < num; i++)
        {
            GameObject newObject = Instantiate(_prefab);
            newObject.SetActive(false);
            _objects.Enqueue(newObject);

            newObject.GetComponent<IGameObjectPooled>().Pool = this;
        }
    }

    public GameObject Get(){
        if (_objects.Count == 0){
            AddObjects(1);
        }
        return _objects.Dequeue();
    }

    public void ReturnToPool(GameObject objectToReturn){
        objectToReturn.SetActive(false);
        _objects.Enqueue(objectToReturn);
    }

    
}
