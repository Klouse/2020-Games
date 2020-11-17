using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        if (other.tag == "Player" && Input.GetKeyDown(KeyCode.E)){
                Player player = other.GetComponent<Player>();
                if (player != null){
                    player.PickUpCoin();
                }
                Destroy(this.gameObject);
        }
    }
}
