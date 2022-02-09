using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashEnemyLogic : MonoBehaviour
{
   SlimeAi slime;
   Player player;
   CharacterControl playerMovement;

   private void OnEnable() {
      slime = FindObjectOfType<SlimeAi>();
      player = FindObjectOfType<Player>();
      playerMovement = FindObjectOfType<CharacterControl>();
   }

   private void OnTriggerEnter(Collider other) {
      var enemyClass = other.gameObject.GetComponent<EnemyBase>();
      if(enemyClass != null && playerMovement.IsFalling()) {
         enemyClass.TakeDamage(1);
      }
   }
}
