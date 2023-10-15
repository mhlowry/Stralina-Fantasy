using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaTransition : MonoBehaviour
{
    //This is literally just where the player ends up after walking out of an area
    [SerializeField] GameObject destinationGate;
    Transform areaSpawnPoint;

    private void Start()
    {
        //Ideally, gates have a child (transform) that any sibiling gate will transport the player to.
        //However, if the gate does not have a complete sibiling- this allows for that tobject's transform to be a "one way gate"
        if (destinationGate.transform.childCount > 0)
            areaSpawnPoint = destinationGate.transform.GetChild(0).gameObject.transform;
        else
            areaSpawnPoint = destinationGate.transform;
    }

    //This is pretty much gonna be the whole code
    //Turns out "this" was a lot more complicated than I thought it was going to be.
    private void OnTriggerEnter(Collider hitTarget)
    {
        GameObject playerObject = hitTarget.gameObject;

        if (hitTarget.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(ChangeArea(playerObject));
        }
    }

    //So the reason we need to disble the player input is because 
    private IEnumerator ChangeArea(GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();
        //disable input for a moment
        player.DisableInput();
        yield return new WaitForSeconds(0.01f);
        playerObj.transform.position = areaSpawnPoint.position;
        yield return new WaitForSeconds(0.1f);
        player.EnableInput();
    }
}