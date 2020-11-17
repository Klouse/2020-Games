using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeZone : MonoBehaviour
{
    private Player _player;
    [SerializeField]
    private int _zoneID = 1;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("Player is NULL");
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            _player.StartGainingStats(_zoneID);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player"){
            _player.StopGainingStats();
        }
    }
    
}
