using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AreaTransition : MonoBehaviour
{
    //This is literally just where the player ends up after walking out of an area
    [SerializeField] GameObject destinationGate;
    public bool isDestination = false;
    private Coroutine teleportingCoroutine;
    Transform areaSpawnPoint;

    private void Start()
    {
        //Ideally, gates have a child (transform) that any sibiling gate will transport the player to.
        //However, if the gate does not have a complete sibiling- this allows for that tobject's transform to be a "one way gate"
        areaSpawnPoint = destinationGate.transform;
    }

    //This is pretty much gonna be the whole code
    //Turns out "this" was a lot more complicated than I thought it was going to be.
    private void OnTriggerEnter(Collider hitTarget)
    {
        GameObject playerObject = hitTarget.gameObject;

        if (hitTarget.gameObject.layer == LayerMask.NameToLayer("Player") && !isDestination)
        {
            if(areaSpawnPoint.GetComponent<AreaTransition>() != null)
                areaSpawnPoint.GetComponent<AreaTransition>().isDestination = true;
            playerObject.GetComponent<Player>().isTeleporting = true;

            if (teleportingCoroutine != null)
                StopCoroutine(teleportingCoroutine);

            teleportingCoroutine = StartCoroutine(ChangeArea(playerObject));
        }
    }

    private void OnTriggerExit(Collider hitTarget)
    {
        if (hitTarget.gameObject.layer == LayerMask.NameToLayer("Player") && isDestination)
            isDestination = false;
    }

    //So the reason we need to disble the player input is because 
    private IEnumerator ChangeArea(GameObject playerObj)
    {
        if (isDestination)
            yield return null;

        Player player = playerObj.GetComponent<Player>();
        //disable input for a moment
        player.DisableInput();
        yield return new WaitForSeconds(0.01f);
        playerObj.transform.position = areaSpawnPoint.position;
        yield return new WaitForSeconds(0.1f);
        player.EnableInput(); player.GetComponent<Player>().isTeleporting = false;
    }
}