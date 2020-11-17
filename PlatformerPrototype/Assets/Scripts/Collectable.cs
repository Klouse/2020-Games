using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private Color _color;
    [SerializeField]
    private int _pointValue = 1;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color",_color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            Player player = other.GetComponent<Player>();
            if (player != null){
                player.awardCoins(_pointValue);
            }
            Destroy(this.gameObject);
        }
    }
}
