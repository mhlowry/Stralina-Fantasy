using System.Collections;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    public float jumpForce = 0.01f;
    public float jumpHeight = 1f;
    private Rigidbody rb;
    private Animator animator;  // Animator component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();  // Get the Animator component
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        StartCoroutine(JumpEverySecond());
    }

    IEnumerator JumpEverySecond()
    {
        while (true)
        {
            Jump();
            yield return new WaitForSeconds(2);
        }
    }

    void Jump()
    {
        // Jump towards tag player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = player.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

        // Always jump towards the player
        Vector3 jumpVector = horizontalDirection * jumpForce + Vector3.up * jumpHeight;
        rb.velocity = jumpVector;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float damage = Attack();
            // Apply damage to the player here. This is just a log for now.
            Debug.Log("Damage to player: " + damage);
        }
    }

    float Attack()
    {
        // Damage value
        return 10;
    }
}
