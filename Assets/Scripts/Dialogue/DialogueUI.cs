using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
   [SerializeField] private GameObject dialogueBox;
   [SerializeField] private TMP_Text textLabel;
   [SerializeField] private DialogueObject testDialogue;
   private TypewriterEffect typewriterEffect;
   private Player player;

   private void Start()
   {
       player = FindObjectOfType<Player>();
       typewriterEffect = GetComponent<TypewriterEffect>();
       CloseDialogueBox();
       ShowDialogue(testDialogue);
   }

   public void ShowDialogue(DialogueObject dialogueObject)
   {
       dialogueBox.SetActive(true);
       StartCoroutine(StepThroughDialogue(dialogueObject));
   }

   private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
   {
       player.DisableInput();
       foreach (string dialogue in dialogueObject.Dialogue)
       {
         yield return typewriterEffect.Run(dialogue, textLabel);
         yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
       }
       CloseDialogueBox();
       player.EnableInput();
   }

   private void CloseDialogueBox()
   {
       dialogueBox.SetActive(false);
       textLabel.text = string.Empty;
   }
}
