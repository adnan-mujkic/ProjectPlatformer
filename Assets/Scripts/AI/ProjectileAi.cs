using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAi : MonoBehaviour
{
   public int DamagePerSecond;
   public int TickDuration;
   public SlimeAi EnemyBase;

   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.tag == "Player") {
         other.gameObject.GetComponent<Player>().StartDamageTick(new DamageModifier() {
            DamagePerSecond = DamagePerSecond,
            TickRate = TickDuration,
            TicksLeft = TickDuration,
            HealPerSecond = 0
         }, gameObject);
      }
      else if(other.gameObject.tag == "Ground") {
         Vector3 newPos = gameObject.transform.position;
         var decalPos = new Vector3(newPos.x, newPos.y - 0.2f, newPos.z);
         var decalRot = EnemyBase.SplatterDecalPrefab.transform.rotation;
         EnemyBase.SlimeDecalsInWorld.Add(Instantiate(EnemyBase.SplatterDecalPrefab, decalPos, decalRot));
         EnemyBase.SlimeDecalsInWorldTimeLeft.Add(4f);
      }
   }
}
