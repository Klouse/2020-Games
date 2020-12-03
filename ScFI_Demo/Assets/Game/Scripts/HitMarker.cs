using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour, IGameObjectPooled
{
    float _lifetime;
    float _maxLifetime = 0.5f;
    private ObjectPooler pool;
    public ObjectPooler Pool{
        get { return pool;}
        set{
            if (pool == null){
                pool = value;
            }else{
                throw new System.Exception("Bad pool use, should only be set once");
            }
        }
    }
     private void OnEnable() {
         _lifetime = 0f;
     }

    // Update is called once per frame
    void Update()
    {
        _lifetime += Time.deltaTime;
        if (_lifetime > _maxLifetime){
            pool.ReturnToPool(this.gameObject);
        }
    }
}

internal interface IGameObjectPooled
{
    ObjectPooler Pool {get; set;}
}
