using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Rock"){
            other.transform.parent = this.transform;
        }
    }
}
