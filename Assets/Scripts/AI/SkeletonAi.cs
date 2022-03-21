using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkeletonAi : MonoBehaviour
{
   public GameObject BonePrefab;
   public Transform ProjectileTransform;
   public int Level;
   public int HP;
   public bool canThrow;
   public bool InsideCamera;
   public Image HpPanel;
   public Image HpBar;
   public List<Image> hpbars;


   float ThrowFrequency;
   Player player;
   bool facingRight;
   bool invincibleForShield;

   private void OnEnable()
   {
      player = FindObjectOfType<Player>();
      ThrowFrequency = 0.3f * Level;
      StartCoroutine(ThrowBones());
      HP = 1 * Level;
      StartCoroutine(CheckIfInsideCamera());
      invincibleForShield = false;
      hpbars = new List<Image>();
      hpbars.Add(HpBar);
      if(HP > 1) {
         for(int i = 1; i < HP; i++) {
            hpbars.Add(Instantiate(HpBar, HpPanel.transform));
         }
      }
   }


   IEnumerator CheckIfInsideCamera()
   {
      while (true)
      {
         if (player.transform.position.x > transform.position.x && !facingRight)
         {
            Flip();
         }
         else if (player.transform.position.x < transform.position.x && facingRight)
         {
            Flip();
         }
         Vector3 posInCamera = Camera.main.WorldToViewportPoint(transform.position);
         if (posInCamera.x < 0 || posInCamera.y < 0 || posInCamera.x > 1 || posInCamera.y > 1)
         {
            InsideCamera = false;
         }
         else
         {
            InsideCamera = true;
         }
         yield return new WaitForSeconds(0.1f);
      }
   }

   IEnumerator ThrowBones()
   {
      while (true)
      {
         if (InsideCamera && canThrow)
         {
            var bone = Instantiate(BonePrefab);
            bone.transform.position = ProjectileTransform.position;
            bone.GetComponent<BoneAi>().SkeletonFacingRight = facingRight;
            if (facingRight)
            {
               bone.GetComponent<Rigidbody2D>().velocity = new Vector2(10, 0);
            }
            else
            {
               bone.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
            }
         }
         yield return new WaitForSeconds(1f / ThrowFrequency);
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.tag == "Enemy" && !other.GetComponent<BoneAi>().SkeletonOwner)
      {
         InflictDamge();
         Destroy(other.gameObject);
      }
   }

   private IEnumerator RestartInvincibility() {
      yield return new WaitForSeconds(0.2f);
      invincibleForShield = false;
   }

   private void InflictDamge() {
      if(HP > 1)
         hpbars[HP - 1].color = Color.clear;
      HP--;
      if(HP <= 0) {
         player.AddScore(10 * Level);
         Destroy(gameObject);
      }
   }

   private void Flip()
   {
      facingRight = !facingRight;
      Vector3 scaleNew = transform.localScale;
      scaleNew.x *= -1;
      transform.localScale = scaleNew;
   }
}
