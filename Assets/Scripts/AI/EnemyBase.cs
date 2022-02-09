using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBase: MonoBehaviour
{
   public GameObject EnemyModel;
   public float Speed;
   int HP;
   internal Player player;
   bool facingRight;
   public bool InsideCamera;
   public bool enableAI;
   public bool playerInsideAttackRange;
   public bool attacking;


   private void OnEnable() {
      Enable();
   }
   private void FixedUpdate() {
      FUpdate();
   }

   internal virtual void Enable() {
      player = FindObjectOfType<Player>();
      StartCoroutine(CheckIfInsideCamera());
   }

   internal virtual void FUpdate() {
      if(!InsideCamera || !enableAI || attacking)
         return;

      if(!attacking && playerInsideAttackRange)
         StartBasicAttack();

      float calculatedSpeed = Speed * Time.deltaTime;
      transform.position += new Vector3(
         (player.transform.position.x >= transform.position.x ? calculatedSpeed : -calculatedSpeed), 0, 0);
   }

   public virtual void TakeDamage(int amount) {
      HP--;
      if(HP <= 0) {
         HP = 0;
         Die();
      }
   }
   public virtual void Die() {
      Destroy(this.gameObject);
   }
   private void Flip() {
      EnemyModel.transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
      facingRight = !facingRight;
   }
   public virtual void StartBasicAttack() {}

   IEnumerator CheckIfInsideCamera() {
      while(true) {
         if(!attacking) {
            if(player.transform.position.x > transform.position.x && !facingRight) {
               Flip();
            } else if(player.transform.position.x < transform.position.x && facingRight) {
               Flip();
            }
            Vector3 posInCamera = Camera.main.WorldToViewportPoint(transform.position);
            if(posInCamera.x < 0 || posInCamera.y < 0 || posInCamera.x > 1 || posInCamera.y > 1) {
               InsideCamera = false;
            } else {
               InsideCamera = true;
            }
         }
         yield return new WaitForSeconds(0.1f);
      }
   }

   private void OnTriggerEnter(Collider other) {
      if(other.gameObject.tag == "Player") {
         if(other.gameObject.transform.position.y < gameObject.transform.position.y + 0.1f) {
            other.gameObject.GetComponent<Player>().TakeDamage(1);
         }
      }
   }
}
