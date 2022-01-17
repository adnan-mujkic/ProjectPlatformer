using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
   public bool dialogueOpen;
   public Text DialogueText;
   public DialogueSO DialogueTrigger;
   Player player;
   public AudioClip CalmMusic;


   private void Start()
   {
      player = FindObjectOfType<Player>();
   }
   void Update()
   {
      if (dialogueOpen)
      {
         if (Input.GetKeyDown(KeyCode.Space))
         {
            if (DialogueTrigger == null)
               return;
            if (DialogueTrigger.NextDialogue == null)
            {
               if(DialogueTrigger.finalBossDialogue)
                  FindObjectOfType<FinalBossAi>().GetComponent<SkeletonAi>().canThrow = true;
               DialogueTrigger = null;
               GetComponent<Animator>().SetTrigger("CloseDialogue");
               FindObjectOfType<CharacterControl>().enabled = true;
               dialogueOpen = false;
            }
            else
            {
               StopAllCoroutines();
               TriggerDialogue(DialogueTrigger.NextDialogue);

            }
         }
      }
   }
   public void TriggerDialogue(DialogueSO DialogueToTrigger)
   {
      FindObjectOfType<CharacterControl>().GetComponent<Animator>().SetBool("Walking", false);
      FindObjectOfType<CharacterControl>().GetComponent<Animator>().SetBool("Jumping", false);
      DialogueTrigger = DialogueToTrigger;
      if (DialogueTrigger != null && DialogueTrigger.TriggersAudio != null)
         StartCoroutine(SwitchAudio(DialogueTrigger.TriggersAudio));
      if (dialogueOpen)
      {
         StartCoroutine(TypeDialogue());
      }
      else
      {
         dialogueOpen = true;
         GetComponent<Animator>().SetTrigger("OpenDialogue");
         StartCoroutine(TypeDialogue());
      }
   }
   IEnumerator TypeDialogue()
   {
      int currentLetterIndex = 0;
      DialogueText.text = "";
      while (DialogueTrigger != null && (currentLetterIndex < DialogueTrigger.DialogueText.Length))
      {
         DialogueText.text += DialogueTrigger.DialogueText[currentLetterIndex];
         currentLetterIndex++;
         yield return new WaitForSeconds(0.01f);
      }
   }

public void SwitchMusic(){
   StartCoroutine(SwitchAudio(CalmMusic));
}
   public IEnumerator SwitchAudio(AudioClip clip)
   {
      var source = FindObjectOfType<AudioSource>();
      source.clip = clip;
      //source.volume = player.Volume;
      source.Play();
      yield return null;
   }
}
