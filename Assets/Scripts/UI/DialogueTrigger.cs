using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
   public DialogueSO DialogueToTrigger;
   public bool dialogueEnabled;
   public int index;

   private void Awake()
   {
      if(Player.DialoguesEnabled == null)
         return;
      for (int i = 0; i < Player.DialoguesEnabled.Length; i++)
      {
         if (Player.DialoguesEnabled[i].index == index)
            dialogueEnabled = Player.DialoguesEnabled[i].enabled;
      }
   }

   private void OnTriggerEnter2D(Collider2D other)
   {
      if (other.tag == "Player" && dialogueEnabled)
      {
         FindObjectOfType<DialogueManager>().TriggerDialogue(DialogueToTrigger);
         FindObjectOfType<Player>().GetComponent<Rigidbody2D>().velocity = Vector2.zero;
         FindObjectOfType<CharacterControl>().enabled = false;
         dialogueEnabled = false;
         for (int i = 0; i < Player.DialoguesEnabled.Length; i++)
         {
            if (Player.DialoguesEnabled[i].index == index)
               Player.DialoguesEnabled[i].enabled = false;
         }
      }
   }
}
