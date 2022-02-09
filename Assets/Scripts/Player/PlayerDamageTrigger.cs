using UnityEngine;

public class PlayerDamageTrigger : MonoBehaviour
{
   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.GetComponent<EnemyBase>() != null)
         Debug.Log("Recive Damage");
   }
}
