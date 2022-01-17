using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Create/Dialogue")]
public class DialogueSO : ScriptableObject
{
   [TextArea(2,5)]
   public string DialogueText;
   public DialogueSO NextDialogue;
   public AudioClip TriggersAudio;
   public bool finalBossDialogue;
}
