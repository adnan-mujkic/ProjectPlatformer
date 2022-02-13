using UnityEngine;

public class EnemyColliderAi : MonoBehaviour
{
   Player player;
   public EnemyBase self;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
   }

   private void OnTriggerEnter(Collider other) {
      if(other.tag == "DamageTrigger" && !self.takingDamage) {
         player.TakeDamage(1);
      }
   }

   public EnemyBase GetSelf() {
      return self;
   }
}
