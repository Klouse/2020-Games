using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal interface IGameObjectPooled {
    GameObjectPooler Pool {get; set;}
    string poolTag {get; set;}
}
