using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaTransition))]
public class ActvateGateMiniObjective : MonoBehaviour
{
    public List<GameObject> objectsToCheck; // List of game objects to check

    AreaTransition areaTransition;

    private void Awake()
    {
        areaTransition = GetComponent<AreaTransition>();
    }

    private void Update()
    {
        // Check if all objects in the list have been destroyed
        bool allDestroyed = true;
        foreach (GameObject obj in objectsToCheck)
        {
            if (obj != null)
            {
                allDestroyed = false;
                break; // If at least one object is not destroyed, break the loop
            }
        }

        // If all objects are destroyed, set isActive to true
        if (allDestroyed)
        {
            areaTransition.ActivateGate();
            this.enabled = false;
            // You can add any additional actions or code here when all objects are destroyed.
        }
    }
}
