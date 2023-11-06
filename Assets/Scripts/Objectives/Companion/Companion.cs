using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Companion : MonoBehaviour
{
    [SerializeField] private float followRadius = 10f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float timeBetweenNewPosition = 2f;
    [SerializeField] private float teleportYThreshold = 1f; // The y-axis difference threshold for teleporting
    [SerializeField] private Vector2 teleportOffsetRange = new Vector2(0.5f, 1.5f); // Range for random offset when teleporting

    private Rigidbody rb;
    private GameObject playerObject;
    private Vector3 targetPosition;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        StartCoroutine(UpdateTargetPositionCoroutine());
    }

    private void Update()
    {
        CheckTeleport();
        MoveWithPlayer();
    }

    private IEnumerator UpdateTargetPositionCoroutine()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, playerObject.transform.position) <= followRadius)
            {
                // Update target position to a new random position within the follow radius
                Vector3 randomDirection = Random.insideUnitSphere * followRadius;
                randomDirection += playerObject.transform.position;
                randomDirection.y = transform.position.y; // Assuming you want the companion to stay at its current y position

                targetPosition = randomDirection;
            }

            // Wait for some time before changing the target position again
            yield return new WaitForSeconds(timeBetweenNewPosition);
        }
    }

    private void MoveWithPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

        // Move towards the target position if within the follow radius
        if (distanceToPlayer <= followRadius)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Keep the y-axis movement to 0 to ensure sliding movement
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
        // If outside the follow radius, move directly towards the player's position
        else
        {
            Vector3 direction = (playerObject.transform.position - transform.position).normalized;
            direction.y = 0; // Keep the y-axis movement to 0 to ensure sliding movement
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    private void CheckTeleport()
    {
        // Check the y-axis difference
        if (Mathf.Abs(transform.position.y - playerObject.transform.position.y) > teleportYThreshold)
        {
            // Teleport to the player with a random offset in x and z, but set the y to the player's y
            float offsetX = Random.Range(-teleportOffsetRange.x, teleportOffsetRange.x);
            float offsetZ = Random.Range(-teleportOffsetRange.y, teleportOffsetRange.y);
            Vector3 newPosition = new Vector3(
                playerObject.transform.position.x + offsetX,
                playerObject.transform.position.y, // Set the y position to the player's y
                playerObject.transform.position.z + offsetZ
            );
            
            // Use transform.position to teleport
            transform.position = newPosition;
        }
    }

}
