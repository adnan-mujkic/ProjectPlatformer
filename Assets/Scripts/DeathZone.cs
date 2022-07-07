using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
   private Vector3 spawnPointPos;

   private void OnEnable() {
      spawnPointPos = GameObject.FindObjectOfType<Player>().PlayerSpawn.transform.position;
   }

   private void OnTriggerEnter(Collider other) {
      if(other.tag == "Player") {
         Player player = other.gameObject.GetComponent<Player>();
         player.TakeDamage(1);
         player.transform.position = spawnPointPos;
      }
   }
}
