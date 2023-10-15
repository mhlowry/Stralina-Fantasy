using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealthPoints = 10;
    [SerializeField] private int expWorth;
    [SerializeField] protected float knockbackMultiplier = 1f;
    protected int curHealthPoints;
    protected bool isChase = true;
    bool isDead = false;
    protected Animator animator;

    //Game components that will be generally needed
    [SerializeField] protected Rigidbody rb;
    private Player playerScript;
    private Rigidbody playerRb;
    protected GameObject playerObject;

    private CinemachineImpulseSource impulseSource;

    protected virtual void Awake()
    {
        //sets whatever object this is on to be put on the "enemy" layer, so the player can attack it.
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        gameObject.layer = enemyLayer;

        //sets enemy's current health to max health on awake
        curHealthPoints = maxHealthPoints;

        //Gets rigidbody
        rb = GetComponent<Rigidbody>();

        //if animator exists, gets animator
        animator = GetComponent<Animator>();

        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerScript = playerObject.GetComponent<SwordMan>();
            //playerRb = playerObject.GetComponent<Rigidbody>();
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public virtual void TakeDamage(int damage, float knockBack, Vector3 direction)
    {
        //add screenshake on impact
        CameraShake.instance.ShakeCamera(impulseSource);

        AudioManager.instance.PlayRandom(new string[] { "impact_1", "impact_2", "impact_3", "impact_4", "impact_5", "impact_6" });
        HitStop.instance.Stop(0.03f); //0.03f

        curHealthPoints -= damage;

        //calculate vector of position relative from the player's position to the enemy's position
        Vector3 enemyPosition = gameObject.transform.position;
        Vector3 playerPosition = playerObject.transform.position;

        // Calculate the direction vector
        Vector3 relativePos = enemyPosition - playerPosition;
        Vector3 normRelative = relativePos.normalized;
        Vector3 normAttack = direction.normalized;

        //adds unit vector of both the direction of the attack, and position relative to the player.  This is to create a dynamic knockback effect
        Vector3 knockbackDir = normRelative + normAttack;

        //Debug.Log("Knockback: " + knockbackDir.normalized);

        rb.AddForce(knockBack * knockbackMultiplier * knockbackDir.normalized, ForceMode.Impulse);
    }

    // Deal damage to player
    public void DealDamage(float damage, float knockback, Vector3 direction)
    {
        if (playerObject != null)
        {
            if (playerScript != null)
            {
                playerScript.TakeDamage(1);
            }
            else
            {
                Debug.LogError("PlayerScript is not found on the player object!");
            }
        }
        else
        {
            Debug.LogError("Player object is not set!");
        }

        // will maybe add knockback later
    }


    protected virtual void Die()
    {
        isDead = true;
        if (animator != null)
        {
            animator.SetTrigger("isDead");
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }

        StartCoroutine(DestroyEnemy());
    }

    public bool GetIsDead() { return isDead; }
    public int GetExpWorth() { return expWorth; }

    //destroys the object after some time has passed
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }

    public float playerDistance()
    {
        return Vector3.Distance(playerObject.transform.position, transform.position);
    }
}
