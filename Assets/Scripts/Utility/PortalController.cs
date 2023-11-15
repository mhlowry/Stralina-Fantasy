using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [SerializeField] private string destinationScene; // Scene to load
    public bool isActive = true;
    GameObject portalVisual;

    private void Awake()
    {
        try
        {
            if (transform.GetChild(0).gameObject)
            {
                portalVisual = transform.GetChild(0).gameObject;
                portalVisual.SetActive(isActive);
            }
        }
        catch
        {
            Debug.Log("Prototype Portal Warning");
        }
    }

    private void OnTriggerEnter(Collider hitTarget)
    {
        if (hitTarget.gameObject.layer == LayerMask.NameToLayer("Player") && isActive)
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

      // This is just to keep the structure of a coroutine
      // We should instead have a transition and do this: yield return new WaitForSeconds(transitionTime);
      yield return null; 
  }


    public void ActivateGate()
    {
        isActive = true;
        portalVisual.SetActive(isActive);
    }
}
