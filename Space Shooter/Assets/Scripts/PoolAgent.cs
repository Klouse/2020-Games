using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAgent : MonoBehaviour, IGameObjectPooled
{
    private GameObjectPooler _pool;
    private string _poolTag = "";

    public string poolTag{
        get {return _poolTag;}
        set {if (_poolTag == ""){
            _poolTag = value;
        }else{
            throw new System.Exception("Bad number usage, pool tag should never change");
        }}
    }

    public GameObjectPooler Pool{
        get{return _pool;}
        set{
            if (_pool == null){
                _pool = value;
            }else{
                throw new System.Exception("Bad Pool usage, pool should only be set once");
            }
        }
    }
}
