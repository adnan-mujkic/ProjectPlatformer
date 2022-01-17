using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTracker : MonoBehaviour
{
   public bool detectedBone;
   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.tag == "Enemy"&& !other.GetComponent<BoneAi>().SkeletonOwner)
      {
         detectedBone = true;
         if (!GetComponent<FinalBossAi>().jumping || GetComponent<Rigidbody2D>().velocity.y == 0)
         {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 6f);
            GetComponent<FinalBossAi>().jumping = true;
         }
      }

   }
}
