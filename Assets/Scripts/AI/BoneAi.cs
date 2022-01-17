using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneAi : MonoBehaviour
{
   public Vector3 posInCamera;
   public bool SkeletonFacingRight;
   public bool SkeletonOwner;

   private void OnEnable() {
      StartCoroutine(CheckIfOffScreen());
      SkeletonOwner = true;
   }
   IEnumerator CheckIfOffScreen()
   {
      while (true)
      {
         yield return new WaitForSeconds(0.1f);
         posInCamera = Camera.main.WorldToViewportPoint(transform.position);
         if (posInCamera.x < 0 || posInCamera.y < 0 || posInCamera.x > 1 || posInCamera.y > 1)
         {
            Destroy(gameObject);
         }
      }
   }
}
