using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private string destinationScene; // Scene to load
    private GameObject portalVisual; // Child object representing the portal

    private void Awake()
    {
        portalVisual = FindPortalVisual();
        if (portalVisual == null)
        {
            Debug.LogError("Portal visual not found!");
        }
    }

    private GameObject FindPortalVisual()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Portal"))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private void OnTriggerEnter(Collider hitTarget)
    {
        // Check if the collider that entered the trigger is the player
        // and if the portal visual is active (visible)
        if (hitTarget.gameObject.layer == LayerMask.NameToLayer("Player") && portalVisual.activeSelf)
        {
            StartCoroutine(TransitionToScene(hitTarget.gameObject));
        }
    }

    private IEnumerator TransitionToScene(GameObject playerObj)
    {
        Player player = playerObj.GetComponent<Player>();

        // Disable input to prevent any further actions during scene loading
        player.DisableInput();

        // Load the destination scene immediately
        SceneManager.LoadScene(destinationScene, LoadSceneMode.Single);

        yield return null; 
    }

    public void ActivateGate(bool isActive)
    {
        if (portalVisual != null)
        {
            portalVisual.SetActive(isActive);
        }
    }
}
