using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossAi : MonoBehaviour
{
   public bool jumping;
    public void UnlockSprint(){
       CharacterControl.SprintEnabled = true;
       CharacterControl.maxJumps = 2;
       FindObjectOfType<DialogueManager>().SwitchMusic();
    }
    private void Update() {
      if(Input.GetKeyDown(KeyCode.Space) && GetComponent<SkeletonAi>().canThrow && !jumping){
         GetComponent<Rigidbody2D>().velocity = new Vector2(0 , 10f);
         jumping = true;
      }
      if(GetComponent<Rigidbody2D>().velocity.y == 0){
         jumping = false;
      }
   }
}
