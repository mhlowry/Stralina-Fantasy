using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private GameObject interactText;
    [SerializeField] private DialogueObject nextDialogueObject;
    [SerializeField] private GameObject shopkeeper;

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

        CheckAndUpdateShopkeeperDialogue();

        // if the player has spoken to the shopkeeper, randomize the shopkeeper dialogue
        if (GameManager.instance.GetDialogueName() != "FE 1-1")
        {
            GameManager.instance.InitializeShopkeeperDialogue(); 
            Debug.Log("Shopkeeper dialogue initialized, dialogue name is: " + GameManager.instance.GetDialogueName());
        }
        else
        {
            Debug.Log("Shopkeeper dialogue not initialized, dialogue name is: " + GameManager.instance.GetDialogueName());
        }

        Debug.Log("This is found here");
    }

    void CheckAndUpdateShopkeeperDialogue()
    {
        DialogueActivator dialogueActivator = shopkeeper.GetComponent<DialogueActivator>();
        if (dialogueActivator != null)
        {
            // Check if the DialogueObject is not "FE 1-1"
            if (dialogueActivator.DialogueObject.name != "FE 1-1")
            {
                GameManager.instance.SetDialogueName("RT 1-1");
            }
        }
        else
        {
            Debug.LogError("Shopkeeper does not have a DialogueActivator component.");
        }
    }


    public DialogueObject DialogueObject 
    {
        get { return dialogueObject; }
    }
}
