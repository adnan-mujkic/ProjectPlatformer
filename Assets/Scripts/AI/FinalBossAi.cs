using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinalBossAi: MonoBehaviour
{
   public Player player;
   public GameObject skeleModel;
   public bool facingRight;
   public Canvas hpCanvas;
   public int HP;
   public HpBarWrapper HpBar;
   public bool jumping;
   private Animator animator;
   public bool vunerable;
   public bool playerParry;
   public Transform[] spawnLocations;
   private int spawnIndex = 0;
   public GameObject teleportVfxObject;
   private bool attacking;

   List<CameraFixator> BossArenas;

   private void OnEnable() {
      player = FindObjectOfType<Player>();
      animator = skeleModel.gameObject.GetComponent<Animator>();
      SetHP(3);
      hpCanvas.gameObject.SetActive(false);

      var cameraFixators = GameObject.FindObjectsOfType<CameraFixator>();
      BossArenas = new List<CameraFixator>();
      foreach(var cam in cameraFixators) {
         if(cam.TriggersBoss)
            BossArenas.Add(cam);
      }

      ModifyBossZones();
   }

   private void Update() {
      if(!attacking && player.gameObject.transform.position.x > gameObject.transform.position.x && !facingRight)
         Flip();
      if(!attacking && player.gameObject.transform.position.x < gameObject.transform.position.x && facingRight)
         Flip();
   }

   public void SetHP(int amount) {
      HP = amount;
      HpBar.UpdateHp(HP);
   }

   public void TakeDamage(int amount) {
      HP -= amount;
      if(HP < 0)
         HP = 0;
      HpBar.UpdateHp(HP);
      if(HP <= 0) {
         if(spawnIndex < spawnLocations.Length - 1) {
            HP = 3;
            HpBar.UpdateHp(HP);
            DeactivateAi();
            FireTeleportVfx();
            StartCoroutine(MoveToNextLocation());
            var am = Camera.main.GetComponent<AudioManager>();
            am.PlayMusic(am.MusicToPlay, true, true, am.TopMusicVolume);
         } else {
            DeactivateAi();
            player.ReturnToSpawn(1);
            StartCoroutine(DieAnim());
         }
      }
   }

   void Flip() {
      skeleModel.transform.localScale = new Vector3(!facingRight ? 1 : -1, 1, 1);
      facingRight = !facingRight;
   }

   public void ActivateAi() {
      Flip();
      hpCanvas.gameObject.SetActive(true);
      vunerable = false;
      StartCoroutine(AttackLoop());
   }

   public void DeactivateAi() {
      hpCanvas.gameObject.SetActive(false);
      vunerable = false;
      StopAllCoroutines();
      skeleModel.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
      ModifyBossZones();
   }

   private IEnumerator MoveToNextLocation() {
      yield return new WaitForSeconds(1f);
      spawnIndex++;
      Vector3 nextSpawn = spawnLocations[spawnIndex].position;
      gameObject.transform.DOMove(nextSpawn, 2f);
      ModifyBossZones();
      player.ReturnToSpawn();
   }

   private IEnumerator AttackLoop() {
      while(true) {
         bool attackOrJump = Random.Range(0, 100) >= 50;
         if(!attackOrJump && Vector3.Distance(transform.position, player.transform.position) <= 4f) {
            attackOrJump = true;
         }

         if(attackOrJump) {
            attacking = true;
            float distance = Vector3.Distance(player.transform.position, transform.position);
            gameObject.transform.DOMoveX(player.transform.position.x, (distance / 4f) - 0.2f);
            animator.SetBool("Moving", true);
            yield return new WaitForSeconds((distance / 4f));
         } else {
            StartCoroutine(Jump(transform.position, new Vector3((transform.position.x + player.transform.position.x)/2f ,transform.position.y + 10f, transform.position.z) , player.transform.position));
            animator.SetBool("Moving", true);
            yield return new WaitForSeconds(0.5f);
         }
         attacking = true;
         animator.SetBool("Moving", false);
         animator.SetTrigger("Attack");
         yield return new WaitForSeconds(0.3f);
         vunerable = true;
         skeleModel.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 100f / 255f, 100f / 255f);

         yield return new WaitForSeconds(0.2f);
         if(Vector3.Distance(player.transform.position, gameObject.transform.position) < 1.8f && !playerParry) {
            player.TakeDamage(2);
         }
         if(playerParry)
            playerParry = false;
         vunerable = false;
         skeleModel.GetComponent<MeshRenderer>().material.color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
         attacking = false;
         yield return new WaitForSeconds(0.4f);
      }
   }

   private IEnumerator Jump(Vector3 startingPos, Vector3 height, Vector3 endPos) {
      transform.position = startingPos;
      float seconds = 0f;
      while(seconds <= 1f) {
         transform.position = CalculateQuadraticBezierPoint(seconds, startingPos, height, endPos);
         seconds += Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }
      transform.position = endPos;
   }

   private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 P0, Vector3 P1, Vector3 P2) {
      float u = 1 - t;
      float tt = t * t;
      float uu = u * u;

      Vector3 P = uu * P0;
      P += 2 * u * t * P1;
      P += tt * P2;

      return P;
   }

   private void ModifyBossZones() {
      foreach(var bossZone in BossArenas) {
         bossZone.ToggleCollision(bossZone.BossArenaIndex == spawnIndex);
      }
   }

   private IEnumerator FireTeleportVfx() {
      teleportVfxObject.SetActive(true);
      teleportVfxObject.GetComponent<Animator>().Play("Idle");
      yield return new WaitForSeconds(1.2f);
      teleportVfxObject.SetActive(false);
   }
   private IEnumerator DieAnim() {
      animator.SetTrigger("Die");
      yield return new WaitForSeconds(1);
      Destroy(gameObject);
   }
}
