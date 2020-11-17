using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkShop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other) {
        if (other.tag == "Player" && Input.GetKey(KeyCode.E)){
            Player player = other.GetComponent<Player>();
            if (player != null){
                if (player._hasCoin){
                    player.PayForGun();
                }else{
                    Debug.Log("GTFO MATE");
                }
            }
        }
    }
}
