using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
   public bool OverTime;
   public int Damage;
   public int TickDuration;
   Player player;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
   }

   private void OnTriggerEnter(Collider collision) {
      if(collision.gameObject.tag == "Player") {
         if(OverTime) {
            player.StartDamageTick(new DamageModifier() {
               DamagePerSecond = Damage,
               TickRate = TickDuration,
               TicksLeft = TickDuration,
               HealPerSecond = 0
            }, Assets.Scripts.Enums.EDamageOverTimeType.Poison);
         } else {
            player.TakeDamage(Damage);
         }
      }
   }
}
