using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryPickup : MonoBehaviour
{
   private void OnTriggerEnter(Collider other) {
      if(other.tag == "Player") {
         other.GetComponent<Player>().AddParry();
         Destroy(gameObject);
      }
   }
}
