using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private GameObject interactText;
    [SerializeField] private DialogueObject nextDialogueObject;

    public void UpdateDialogueObject(DialogueObject newDialogueObject)
    {
        dialogueObject = newDialogueObject;
    }

    public void SetNextDialogueObject(DialogueObject newNextDialogueObject)
    {
        nextDialogueObject = newNextDialogueObject;
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            player.Interactable = this;
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && other.TryGetComponent(out Player player))
        {
            if (player.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
            {
                player.Interactable = null;
                interactText.SetActive(false);
            }
        }
    }

    public void Interact(Player player)
    {
        interactText.SetActive(false);

        foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
        {
            if (responseEvents.DialogueObject == dialogueObject)
            {
                player.DialogueUI.AddResponseEvent(responseEvents.Events);
                break;
            }
        }

        player.DialogueUI.ShowDialogue(dialogueObject);
        dialogueObject = nextDialogueObject;

        Debug.Log("This is found here");
    }
}
