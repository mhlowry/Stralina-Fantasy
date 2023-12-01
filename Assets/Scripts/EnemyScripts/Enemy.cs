using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    private WaveSpawner waveSpawner;
    [SerializeField] protected int maxHealthPoints = 10;
    [SerializeField] protected int expWorth;
    [SerializeField] protected float knockbackMultiplier = 1f;
    protected int curHealthPoints;
    protected bool isChase = true;
    protected bool isDead = false;
    protected Animator animator;

    //Game components that will be generally needed
    protected Rigidbody rb;
    protected Collider selfCollider;
    protected Player playerScript;
    protected GameObject playerObject;
    protected GameObject companionObject; 
    protected GameObject currentTarget;
    protected Companion companionScript;

    //dropping items and whatnot
    [SerializeField] List<GameObject> itemDrops;

    [SerializeField] protected GameObject gfxObject;
    protected CinemachineImpulseSource impulseSource;
    public static event System.Action OnEnemyDestroyed;

    // UpdateTarget variables
    private const float InterestDecayRate = 0.1f; // The rate at which interest in the current target decays
    private const float InterestThreshold = 0.5f; // The threshold at which the enemy considers switching targets
    private float currentInterestLevel = 1f; // The current interest level in the current target
    private float lastDistanceToCurrentTarget = Mathf.Infinity; // Last recorded distance to the current target

    protected virtual void Awake()
    {
        //sets whatever object this is on to be put on the "enemy" layer, so the player can attack it.
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        gameObject.layer = enemyLayer;

        //waveSpawner = GetComponentInParent<WaveSpawner>();

        //Had to change the way this works because otherwise mimics don't work in waves
        try
        {
            waveSpawner = GameObject.Find("WaveSpawner").GetComponent<WaveSpawner>();
        }
        catch
        { }

        //sets enemy's current health to max health on awake
        curHealthPoints = maxHealthPoints;

        //Gets rigidbody && Collider
        rb = GetComponent<Rigidbody>();
        selfCollider = GetComponent<Collider>();

        //if animator exists, gets animator
        if (gfxObject != null )
            animator = gfxObject.GetComponent<Animator>();

        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerScript = playerObject.GetComponent<Player>();
            //playerRb = playerObject.GetComponent<Rigidbody>();
        }

        // Attempt to find Companion in the scene
        companionObject = GameObject.FindGameObjectWithTag("Companion");

        if (companionObject != null)
        {
            companionScript = companionObject.GetComponent<Companion>();
        }

        // Initially set the player as the current target
        currentTarget = playerObject;

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        // Check if the companion is still active/exists in the scene
        if (companionObject == null || !companionObject.activeInHierarchy)
        {
            // Attempt to find Companion again or reset to player
            companionObject = GameObject.FindGameObjectWithTag("Companion");
            if (companionObject == null)
            {
                currentTarget = playerObject;
            }
        }

        // Update the target selection
        UpdateTarget();
    }

/*    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 300)
        {
            rb.velocity = rb.velocity.normalized * 300;
        }
    }*/

    protected void UpdateTarget()
    {
        // Only update the target if there is a companion in the scene
        if (!companionObject)
        {
            currentTarget = playerObject;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
        float distanceToCompanion = Vector3.Distance(transform.position, companionObject.transform.position);
        GameObject closest = distanceToPlayer <= distanceToCompanion ? playerObject : companionObject;
        float distanceToClosest = Mathf.Min(distanceToPlayer, distanceToCompanion);

        // Check if the enemy has lost interest in its current target or the other target is significantly closer
        if (currentTarget == closest)
        {
            // If closer to the target than last time, increase interest, otherwise decay it
            if (distanceToClosest < lastDistanceToCurrentTarget)
            {
                currentInterestLevel = Mathf.Clamp01(currentInterestLevel + (lastDistanceToCurrentTarget - distanceToClosest) * InterestDecayRate);
            }
            else
            {
                currentInterestLevel -= InterestDecayRate * Time.deltaTime;
            }
        }
        else
        {
            currentInterestLevel -= InterestDecayRate * Time.deltaTime;
        }

        // Consider switching targets if the interest level is below the threshold or if the other target is much closer
        if (currentInterestLevel < InterestThreshold || distanceToClosest < lastDistanceToCurrentTarget * 0.5f)
        {
            // Randomly decide to switch targets based on a dice roll
            if (Random.value < currentInterestLevel)
            {
                currentTarget = closest;
                currentInterestLevel = 1f; // Reset interest level for the new target
            }
        }

        // Update the last distance for the next check
        lastDistanceToCurrentTarget = distanceToClosest;
    }

    public GameObject getCurrentTarget()
    {
        return currentTarget;
    }

    public virtual void TakeDamage(int damage, float knockBack, Vector3 direction)
    {
        // Don't take damage if you're dead
        if (isDead)
            return;

        // Add screenshake on impact
        CameraShake.instance.ShakeCamera(impulseSource);

        // Play impact sound
        AudioManager.instance.PlayRandom(new string[] { "impact_1", "impact_2", "impact_3", "impact_4", "impact_5", "impact_6" });

        // Hit stop effect
        HitStop.instance.Stop(0.04f);

        // Reduce current health
        curHealthPoints -= damage;

        // Calculate vector of position relative from the current target's position to the enemy's position
        Vector3 enemyPosition = gameObject.transform.position;
        Vector3 targetPosition = currentTarget.transform.position;

        // Calculate the direction vector
        Vector3 relativePos = enemyPosition - targetPosition;
        Vector3 normRelative = relativePos.normalized;
        Vector3 normAttack = direction.normalized;

        // Adds unit vector of both the direction of the attack, and position relative to the current target.
        // This is to create a dynamic knockback effect
        Vector3 knockbackDir = normRelative + normAttack;

        // Apply knockback force
        rb.AddForce(knockBack * knockbackMultiplier * knockbackDir.normalized, ForceMode.Impulse);
    }


   // Deal damage to player or companion
    protected virtual void DealDamage(int damage, float knockback, GameObject collider)
    {
        if (collider == playerObject && playerScript != null)
        {
            playerScript.TakeDamage(damage, knockback, transform.position);
        }
        else if (companionObject && collider == companionObject && companionScript != null)
        {
            companionScript.TakeDamage(damage, knockback, transform.position);
        }
        else
        {
            Debug.LogError("Current target script is not found or current target is not set!");
        }
    }


    protected virtual void Die()
    {
        if (waveSpawner != null) waveSpawner.waves[waveSpawner.currentWaveIndex].enemiesLeft--;

        isDead = true;
        playerScript.GainExp(expWorth);

        if (animator != null)
        {
            animator.SetTrigger("died");
            animator.SetBool("isDead", true);
        }

        StartCoroutine(DestroyEnemy());
    }

    public bool GetIsDead() { return isDead; }
    public int GetExpWorth() { return expWorth; }

    //Now with new spawn item feature!
    //destroys the object after some time has passed
    IEnumerator DestroyEnemy()
    {
        //wait half a second before dropping item
        foreach (GameObject item in itemDrops)
        {
            Item itemScript = item.GetComponent<Item>();
            if ( 1 == Random.Range(1, itemScript.GetRarity()))
            {
                Instantiate(item, gameObject.transform.position, gameObject.transform.rotation);
                break;
            }
        }
        
        //wait like 2ish seconds before destorying the object
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
        OnEnemyDestroyed?.Invoke();
    }

    public float targetDistance()
    {
        // sneakily calculates the distance from the current target instead of always the player 
        // mostly this is to not mess up the projectile calculations 
        if (currentTarget == null)
            return Vector3.Distance(playerObject.transform.position, transform.position);
        else  return Vector3.Distance(currentTarget.transform.position, transform.position);
    }
}
