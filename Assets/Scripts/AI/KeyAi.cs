using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyAi : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
       if(other.tag == "Player"){
          FindObjectOfType<Player>().AddKey();
          Destroy(gameObject);
       }
    }
}
