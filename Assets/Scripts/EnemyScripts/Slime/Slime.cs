using UnityEngine;

public class Slime : Enemy
{
    public float moveSpeed = 5f;
    public LayerMask groundLayer;  // LayerMask to identify ground objects
    private Camera mainCamera;

    void Start()
    {
        Awake();
    }

    void FixedUpdate()
    {
        SlideTowardsPlayer();
    }

    void Update()
    {
        FaceCamera();
    }

    private void SlideTowardsPlayer()
    {
        Debug.Log("Player is equal to null? " + (playerObject == null));
        // Move towards the player only if the player exists and the slime is grounded
        if (playerObject != null && aggroRange(5))
        {
            Vector3 direction = playerObject.transform.position - transform.position;
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
            rb.velocity = horizontalDirection * moveSpeed;
        }
    }

    private void FaceCamera()
    {
        // Ensure the slime always faces the camera
        if (mainCamera != null)
        {
            Vector3 lookDirection = mainCamera.transform.position - transform.position;
            lookDirection.y = 0;  // Maintain upright orientation
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }

    private bool IsGrounded()
    {
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer));
        // Check if the slime is grounded using raycasting
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
    }
}
