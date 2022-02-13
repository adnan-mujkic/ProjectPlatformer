using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBase: MonoBehaviour
{
   [System.NonSerialized]
   public bool facingRight;

   [System.NonSerialized]
   public bool InsideCamera;

   [System.NonSerialized]
   public bool playerInsideAttackRange;

   [System.NonSerialized]
   public bool attacking;

   [System.NonSerialized]
   public bool takingDamage;


   public GameObject EnemyModel;
   public float Speed;
   int HP;
   internal Player player;
   public bool enableAI;
   public Vector3[] WalkPoints;
   int WalkpointsCounter;


   private void OnEnable() {
      Enable();
   }
   private void FixedUpdate() {
      FUpdate();
   }

   internal virtual void Enable() {
      player = FindObjectOfType<Player>();
      StartCoroutine(CheckIfInsideCamera());
      if(WalkPoints.Length > 0) {
         WalkpointsCounter = 0;
         StartCoroutine(WalkAround());
      }
   }

   internal virtual void FUpdate() {
      if(!InsideCamera || !enableAI || attacking)
         return;

      if(!attacking && playerInsideAttackRange)
         StartBasicAttack();
   }

   public virtual void TakeDamage(int amount) {
      takingDamage = true;
      HP--;
      if(HP <= 0) {
         HP = 0;
         Die();
      }
      takingDamage = false;
   }
   public virtual void Die() {
      Destroy(this.gameObject);
   }
   private void Flip() {
      EnemyModel.transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
      facingRight = !facingRight;
   }
   public virtual void StartBasicAttack() { }

   IEnumerator CheckIfInsideCamera() {
      while(true) {
         if(!attacking && WalkPoints.Length <= 1) {
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
   IEnumerator WalkAround() {
      transform.position = WalkPoints[0];
      WalkpointsCounter++;
      while(gameObject.activeInHierarchy) {
         Vector3 nextPosition = WalkPoints[WalkpointsCounter];
         Vector3 currentPosition = transform.position;
         if(facingRight && currentPosition.x > nextPosition.x)
            Flip();
         else if(!facingRight && currentPosition.x < nextPosition.x)
            Flip();
         while(System.Math.Round(transform.position.x, 1) != System.Math.Round(nextPosition.x, 1)) {
            if(!attacking)
               transform.position = new Vector3(facingRight? transform.position.x + (Speed * Time.deltaTime) : transform.position.x - (Speed * Time.deltaTime), transform.position.y, transform.position.z);
            yield return new WaitForFixedUpdate();
         }
         bool backwards = false;
         if(WalkpointsCounter == WalkPoints.Length - 1)
            backwards = true;
         else if(WalkpointsCounter == 0)
            backwards = false;

         WalkpointsCounter = backwards ? WalkpointsCounter - 1 : WalkpointsCounter + 1;
      }
   }
}
