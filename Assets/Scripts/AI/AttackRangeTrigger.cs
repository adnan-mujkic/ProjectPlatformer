using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeTrigger : MonoBehaviour
{
   public EnemyBase enemy;

   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.tag == "Player") {
         enemy.playerInsideAttackRange = true;
         enemy.StartBasicAttack();
      }
   }

   private void OnTriggerExit(Collider other) {
      if(other.gameObject.tag == "Player") {
         enemy.playerInsideAttackRange = false;
      }
   }
}
