using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAi : EnemyBase
{
   public GameObject SplatterDecalPrefab;
   public List<GameObject> SlimeDecalsInWorld;
   public List<float> SlimeDecalsInWorldTimeLeft;

   internal override void Enable() {
      base.Enable();
      SlimeDecalsInWorld = new List<GameObject>();
      SlimeDecalsInWorldTimeLeft = new List<float>();
      StartCoroutine(CheckDecals(0.5f));
   }

   public override void StartBasicAttack() {
      base.StartBasicAttack();

      if(playerInsideAttackRange && !attacking)
         StartCoroutine(TriggerAttack());
   }

   IEnumerator TriggerAttack() {
      attacking = true;

      //Anticipation
      transform.DOShakePosition(1f, 0.07f, 30, 90f, false, false);
      float waitSeconds = 0f;
      while(waitSeconds < 1f) {
         waitSeconds += Time.deltaTime;
         yield return new WaitForEndOfFrame();
      }

      //Lift itself
      if(playerInsideAttackRange) {
         Vector3 liftPos = new Vector3(player.transform.position.x,
                                       player.transform.position.y + 2f,
                                       player.transform.position.z);

         transform.DOMove(liftPos, 0.5f);

         //Wait for drop
         yield return new WaitForSeconds(1f);

         //Drop
         waitSeconds = 0.2f;
         Vector3 currentPos = transform.position;
         Vector3 newPos = new Vector3(liftPos.x, liftPos.y - 2f, liftPos.z);
         while(waitSeconds > 0f) {
            waitSeconds -= Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos, newPos, 1f - (waitSeconds * 5f));
            yield return new WaitForEndOfFrame();
         }
         transform.position = newPos;

         var decalPos = new Vector3(newPos.x, newPos.y - 0.5f, newPos.z);
         var decalRot = SplatterDecalPrefab.transform.rotation;
         SlimeDecalsInWorld.Add(Instantiate(SplatterDecalPrefab, decalPos, decalRot));
         SlimeDecalsInWorldTimeLeft.Add(4f);
      }
      
      //Cooldown
      yield return new WaitForSeconds(2f);
      attacking = false;
   }

   IEnumerator CheckDecals(float timeToWait) {
      while(true) {
         if(SlimeDecalsInWorld.Count > 0) {
            for(int i = 0; i < SlimeDecalsInWorldTimeLeft.Count; i++) {
               SlimeDecalsInWorldTimeLeft[i] -= timeToWait;
               if(SlimeDecalsInWorldTimeLeft[i] <= 0f) {
                  Destroy(SlimeDecalsInWorld[i]);
                  SlimeDecalsInWorld.RemoveAt(i);
                  SlimeDecalsInWorldTimeLeft.RemoveAt(i);
                  i--;
               }
            }
         }
         yield return new WaitForSeconds(timeToWait);
      }
   }
}
