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
        animator.SetBool("IsJumping", true);  // Set IsJumping to true to trigger jump animation

        Vector3 horizontalDirection = new Vector3(Random.Range(-0.05f, 0.05f), 0, Random.Range(-0.05f, 0.05f)).normalized;
        Vector3 jumpVector = horizontalDirection * jumpForce + Vector3.up * jumpHeight;

        rb.velocity = jumpVector;

        animator.SetBool("IsJumping", false);  // Reset to idle after the jump
    }
}
