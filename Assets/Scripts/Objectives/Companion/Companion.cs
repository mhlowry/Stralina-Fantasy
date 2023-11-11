using System;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Companion : MonoBehaviour
{
    [SerializeField] private float followRadius = 10f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float timeBetweenNewPosition = 2f;
    [SerializeField] private float teleportYThreshold = 5f; // The y-axis difference threshold for teleporting
    [SerializeField] private Vector2 teleportOffsetRange = new Vector2(0.5f, 1.5f); // Range for random offset when teleporting
    [SerializeField] private int maxHealth = 25;
    [SerializeField] private int curHealth;
    [SerializeField] private bool disableMovement = false;
    
    float timeofHit;
    float defenseScale = 1f;

    // For the blinking effect when taking damage
    private MeshRenderer meshRenderer; // delete when we change to sprite

    private Rigidbody rb;
    private Vector3 targetPosition;
    private ResourceBar healthBar;
    public static event Action OnCompanionDeath;
    protected GameObject playerObject;
    protected GameObject companionObject; 
    protected Player playerScript;
    protected Companion companionScript;
    private GameOverMenu gameOverMenu;


    private void Awake()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();

        gameOverMenu = FindObjectOfType<GameOverMenu>();

        try
        {
            healthBar = GameObject.Find("Companion Health").GetComponent<ResourceBar>();
        }
        catch { }
    }

    private void Start()
    {   
        healthBar.SetMaxResource(maxHealth);
        curHealth = maxHealth; // Initialize the companion's health
        meshRenderer = GetComponentInChildren<MeshRenderer>(); // Get the meshRenderer component for the blinking effect

        StartCoroutine(UpdateTargetPositionCoroutine());
    }

    private void Update()
    {
        CheckTeleport();
        MoveWithPlayer();
    }
    
    public virtual void TakeDamage(int damage, float knockBack, Vector3 enemyPosition)
    {
        //adjust numbers
        timeofHit = Time.time;
        curHealth -= (int)(damage * (2 - defenseScale));

        //begin the blinking effect
        StartCoroutine(BlinkEffect());

        //update health bar
        healthBar.SetResource(curHealth);

        if (curHealth <= 0)
            Die();
    }

    // When sprite is made for companion, either delete or change to sprite renderer
    IEnumerator BlinkEffect()
    {
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer is not assigned.");
            yield break; // Exit the coroutine if meshRenderer is null
        }

        // Blink the mesh renderer to indicate damage taken
        for (int i = 0; i < 10; i++) // Blink for a set number of times
        {
            meshRenderer.enabled = !meshRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        meshRenderer.enabled = true; // Ensure the mesh is enabled after blinking
    }
    private void Die()
    {
      // Temporary: companion just becomes invisible
      // But also game over happens anyway so it doesn't really matter what happens to the companion
      // I'll just make him disappear since I am the dev which means I am God
      if (meshRenderer != null)
      {
          meshRenderer.enabled = false; // be gone!
          // This will probably change to spriteRenderer
      }
      // TODO: Add death animations or effects here

      if (gameOverMenu != null)
      {
          gameOverMenu.SetGameOverText("Your companion died!");
      }

      OnCompanionDeath?.Invoke();
    }

    private IEnumerator UpdateTargetPositionCoroutine()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, playerObject.transform.position) <= followRadius)
            {
                // Update target position to a new random position within the follow radius
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * followRadius;
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
        if (!disableMovement)
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
    }

    private void CheckTeleport()
    {
        // Check the y-axis difference
        if (Mathf.Abs(transform.position.y - playerObject.transform.position.y) > teleportYThreshold)
        {
            // Teleport to the player with a random offset in x and z, but set the y to the player's y
            float offsetX = UnityEngine.Random.Range(-teleportOffsetRange.x, teleportOffsetRange.x);
            float offsetZ = UnityEngine.Random.Range(-teleportOffsetRange.y, teleportOffsetRange.y);
            Vector3 newPosition = new Vector3(
                playerObject.transform.position.x + offsetX,
                playerObject.transform.position.y, // Set the y position to the player's y
                playerObject.transform.position.z + offsetZ
            );
            
            // Use transform.position to teleport
            transform.position = newPosition;
        }
    }
    public int getCompanionHealth()
    {
      return curHealth;
    }
}
