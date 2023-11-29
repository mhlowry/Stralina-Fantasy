using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class DialogueUI : MonoBehaviour
{
   [SerializeField] private GameObject dialogueBox;
   [SerializeField] private TMP_Text textLabel;

   public bool IsOpen { get; private set; }
   
   private ResponseHandler responseHandler;
   private TypewriterEffect typewriterEffect;
   private Player player;

   PlayerInput playerInput;

   void Awake()
   {
       playerInput = GameObject.Find("PlayerInput").GetComponent<PlayerInput>();
   }


   private void Start()
   {
       player = FindObjectOfType<Player>();
       typewriterEffect = GetComponent<TypewriterEffect>();
       responseHandler = GetComponent<ResponseHandler>();
       CloseDialogueBox();
   }

   public void ShowDialogue(DialogueObject dialogueObject)
   {
       IsOpen = true;
       dialogueBox.SetActive(true);
       StartCoroutine(StepThroughDialogue(dialogueObject));
   }

   public void AddResponseEvent(ResponseEvent[] responseEvents)
   {
       responseHandler.AddResponseEvents(responseEvents);
   }

   private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        player.DisableInput();
        playerInput.SwitchCurrentActionMap("UI");

        for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
        {
            string dialogue = dialogueObject.Dialogue[i];

            yield return RunTypingEffect(dialogue);

            textLabel.text = dialogue;

            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

            yield return null;
            yield return new WaitUntil(() => Keyboard.current[Key.Space].wasPressedThisFrame 
                             || (Gamepad.current != null && 
                                 (Gamepad.current[UnityEngine.InputSystem.LowLevel.GamepadButton.East].wasPressedThisFrame
                                  || Gamepad.current[UnityEngine.InputSystem.LowLevel.GamepadButton.West].wasPressedThisFrame)));

        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            CloseDialogueBox();
            playerInput.SwitchCurrentActionMap("Gameplay");
            player.EnableInput();
        }
   }

   private IEnumerator RunTypingEffect(string dialogue)
   {
        typewriterEffect.Run(dialogue, textLabel);

        while (typewriterEffect.IsRunning)
        {
            yield return null;

            if (Keyboard.current[Key.Space].wasPressedThisFrame 
                || (Gamepad.current != null && 
                    (Gamepad.current[UnityEngine.InputSystem.LowLevel.GamepadButton.East].wasPressedThisFrame
                    || Gamepad.current[UnityEngine.InputSystem.LowLevel.GamepadButton.West].wasPressedThisFrame)))
            {
                typewriterEffect.Stop();
                textLabel.text = dialogue;
            }

        }
   }

   public void CloseDialogueBox()
   {
        IsOpen = false;
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
   }
}
