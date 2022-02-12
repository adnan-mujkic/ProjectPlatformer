using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
   public int DamagePerSecond;
   public int TickDuration;

   private void OnTriggerEnter(Collider collision) {
      if(collision.gameObject.tag == "Player") {
         FindObjectOfType<Player>().StartDamageTick(new DamageModifier() {
            DamagePerSecond = DamagePerSecond,
            TickRate = TickDuration,
            TicksLeft = TickDuration,
            HealPerSecond = 0
         }, Assets.Scripts.Enums.EDamageOverTimeType.Poison);
      }
   }
}
