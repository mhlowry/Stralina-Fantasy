using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
   [SerializeField] private GameObject dialogueBox;
   [SerializeField] private TMP_Text textLabel;
   [SerializeField] private DialogueObject testDialogue;
   
   private ResponseHandler responseHandler;
   private TypewriterEffect typewriterEffect;
   private Player player;

   private void Start()
   {
       player = FindObjectOfType<Player>();
       typewriterEffect = GetComponent<TypewriterEffect>();
       responseHandler = GetComponent<ResponseHandler>();
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

       for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
       {
        string dialogue = dialogueObject.Dialogue[i];
        yield return typewriterEffect.Run(dialogue, textLabel);

        if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
       }

       if (dialogueObject.HasResponses)
         {
              responseHandler.ShowResponses(dialogueObject.Responses);
         }
         else
         {
              CloseDialogueBox();
              player.EnableInput();
         }
   }

   private void CloseDialogueBox()
   {
       dialogueBox.SetActive(false);
       textLabel.text = string.Empty;
   }
}
