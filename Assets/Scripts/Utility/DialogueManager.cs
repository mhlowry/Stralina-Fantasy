using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dialogue Manager that holds references to all DialogueObjects
public class DialogueManager : MonoBehaviour
{
    public DialogueObject[] allDialogues; // Populate this in the inspector

    public DialogueObject GetDialogueByName(string name)
    {
        foreach (var dialogue in allDialogues)
        {
            if (dialogue.name == name)
                return dialogue;
        }
        return null;
    }
}
