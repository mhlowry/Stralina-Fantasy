using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueResponseEvents))]
public class DialogueResponseEventsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueResponseEvents responseEvents = (DialogueResponseEvents) target;

        if (GUILayout.Button("You have to press this to update the events bro"))
        {
            responseEvents.OnValidate();
        }
     }
}
