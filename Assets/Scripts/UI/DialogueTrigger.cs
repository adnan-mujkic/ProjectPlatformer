using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
   public DialogueSO DialogueToTrigger;
   public bool enabled;
   public int index;

   private void Awake()
   {
      if(Player.DialoguesEnabled == null)
         return;
      for (int i = 0; i < Player.DialoguesEnabled.Length; i++)
      {
         if (Player.DialoguesEnabled[i].index == index)
            enabled = Player.DialoguesEnabled[i].enabled;
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.tag == "Player" && enabled)
      {
         FindObjectOfType<DialogueManager>().TriggerDialogue(DialogueToTrigger);
         FindObjectOfType<Player>().GetComponent<Rigidbody2D>().velocity = Vector2.zero;
         FindObjectOfType<CharacterControl>().enabled = false;
         enabled = false;
         for (int i = 0; i < Player.DialoguesEnabled.Length; i++)
         {
            if (Player.DialoguesEnabled[i].index == index)
               Player.DialoguesEnabled[i].enabled = false;
         }
      }
   }
}
