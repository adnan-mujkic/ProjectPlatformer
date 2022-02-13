using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashEnemyLogic : MonoBehaviour
{
   Player player;
   CharacterControl playerMovement;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
      playerMovement = FindObjectOfType<CharacterControl>();
   }

   private void OnTriggerEnter(Collider other) {
      var enemyClass = other.GetComponent<EnemyColliderAi>();
      if(enemyClass != null && playerMovement.IsFalling()) {
         enemyClass.GetSelf().TakeDamage(1);
      }
   }
}
