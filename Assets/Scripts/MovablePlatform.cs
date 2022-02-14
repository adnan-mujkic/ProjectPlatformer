using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
   public Vector3[] MovePoints;
   public float Speed;
   public float Linger;
   int movePointsCounter;

   private void OnEnable() {
      transform.position = MovePoints[0];
      movePointsCounter = 1;
      StartCoroutine(MovePlatform());
   }

   private void OnDisable() {
      StopAllCoroutines();
   }

   IEnumerator MovePlatform() {
      while(gameObject.activeInHierarchy) {
         Vector3 nextPosition = MovePoints[movePointsCounter];
         Vector3 currentPosition = transform.position;
         float neededTime = Vector3.Distance(currentPosition, nextPosition) / Speed;
         float timer = 0f;
         while(timer <= neededTime) {
            transform.position = Vector3.Lerp(currentPosition, nextPosition, timer / neededTime);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
         }
         bool backwards = false;
         if(movePointsCounter == MovePoints.Length - 1)
            backwards = true;
         else if(movePointsCounter == 0)
            backwards = false;

         movePointsCounter = backwards ? movePointsCounter - 1 : movePointsCounter + 1;
         yield return new WaitForSeconds(Linger);
      }
   }
}
