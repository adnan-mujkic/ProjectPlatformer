using UnityEngine;

public class HealthPickup : MonoBehaviour
{
   public int amount;

   private void OnTriggerEnter(Collider other) {
      if(other.tag == "Player") {
         other.GetComponent<Player>().AddHp(amount);
         Destroy(gameObject);
      }
   }
}
