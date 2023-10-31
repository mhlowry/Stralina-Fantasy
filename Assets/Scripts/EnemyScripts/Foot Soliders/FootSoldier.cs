using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

//FootSoldier is an enemy class that has universally cool movement and coordination..ish?
public class FootSoldier : Enemy
{
    private Coroutine disableMoveCoroutine;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] public float aggroDistance = 10f;
    [SerializeField] protected float attackDistance = 1f;
    [SerializeField] private float strafeDistance = 3f;
    private float curStrafe;

    [SerializeField, Range(0, 14)] protected int attackPower = 1;
    [SerializeField] protected float knockback = 1f;

    [SerializeField] protected float attackStartup = 1f;
    protected float nextDamageTime = 0;

    [SerializeField] private float boidAwarenessRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] protected float damageInterval = 1f; // in seconds
    [SerializeField] protected float damageIntRandomMin = 0f;
    [SerializeField] protected float damageIntRandomMax = 0f;
    [SerializeField] private float keepEnemiesForSeconds = 2f;
    private float timelastChecked;

    private float changeStrafeInterval;
    private float lastStrafeChange;

    private List<Transform> nearbyEnemies = new List<Transform>();

    private Vector3 movementLine;
    protected Vector3 direction;

    protected float distanceFromPlayer = 999f;

    protected bool hitMidAttack = false;

    protected bool inAttackRange = false;
    protected bool inAggroRange = false;

    protected bool isAttacking = false;

    protected bool canAttack = true;
    protected bool canMove = true;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("FootSoldier");
        strafeDistance += Random.Range(-0.5f, 0.5f);
        curStrafe = strafeDistance;
        changeStrafeInterval = Random.Range(2f, 4f);
    }

    protected virtual void Update()
    {
        distanceFromPlayer = playerDistance();
        inAggroRange = distanceFromPlayer <= aggroDistance;
        inAttackRange = distanceFromPlayer <= attackDistance;

        direction = playerObject.transform.position - transform.position;

        animator.SetFloat("walkSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));

        //set this object to look at the player at any given point in time on the horizontal plane
        transform.LookAt(new Vector3(playerObject.transform.position.x, transform.position.y, playerObject.transform.position.z));

        //refresh enemies in range for seconds
        if (Time.time - timelastChecked > keepEnemiesForSeconds)
            FindNearbyEnemies();

        //change the strafe distance every so often just for kicks
        if (Time.time - lastStrafeChange > changeStrafeInterval)
        {
            lastStrafeChange = Time.time;
            curStrafe = strafeDistance + Random.Range(-1.5f, 0.5f);
        }

        if (nextDamageTime <= Time.time && !canAttack)
            canAttack = true;

        /*        if (playerObject != null && inAggroRange && canMove && !isDead)
                {
                    NavigateCombat();
                }*/
    }

    //The goal, right, is to have a basic "smart" foot soldier that will strafe at a distance if they can't attack
    protected virtual void NavigateCombat()
    {
        Vector3 playerDirection = playerObject.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(playerDirection.x, 0, playerDirection.z).normalized;

        // Calculate a combined vector for nearby enemies
        Vector3 combinedDirection = horizontalDirection;

        //strafing state when foot soldier can't attack
        // If in strafe distance, avoid moving towards the player
        if (distanceFromPlayer >= curStrafe - 0.5 && distanceFromPlayer <= curStrafe + 0.5 && !canAttack)
        {
            // Do not move towards the player
            combinedDirection -= new Vector3(playerDirection.x, 0, playerDirection.z).normalized;
        }

        // Normalize the combined direction vector
        combinedDirection = combinedDirection.normalized;

        if (distanceFromPlayer < curStrafe && !canAttack)
        {
            // Reverse the direction when too close
            combinedDirection = -combinedDirection;
        }

        if (nearbyEnemies.Count > 0)
        {
            foreach (Transform t in nearbyEnemies)
            {
                Vector3 addToDirection = transform.position - t.position;
                Vector3 addHorizDirection = new Vector3(addToDirection.x, 0, addToDirection.z).normalized;
                combinedDirection += addHorizDirection;
            }
        }

        // Apply the combined direction to the velocity
        rb.velocity = new Vector3(combinedDirection.x * moveSpeed, rb.velocity.y, combinedDirection.z * moveSpeed);

        movementLine = combinedDirection;
    }

    protected void FindNearbyEnemies()
    {
        timelastChecked = Time.time;
        nearbyEnemies.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, boidAwarenessRadius, enemyLayer);

        foreach (Collider collider in colliders)
        {
            if (collider.transform != transform)
            {
                nearbyEnemies.Add(collider.transform);
            }
        }
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //start the disablemove so it doesn't start mid combo
        if (disableMoveCoroutine != null)
            StopCoroutine(disableMoveCoroutine);

        disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(2f));

        if (isAttacking)
            hitMidAttack = true;

        //inflict damage
        base.TakeDamage(damage, knockback, direction);

        //die if dead
        //make sure they're not already dying, prevent from calling "die" twice
        if (curHealthPoints <= 0 && !animator.GetBool("isDead"))
        {
            //Debug.Log(gameObject.name + " Fucking Died");
            canMove = false;
            Die();
            StopCoroutine(disableMoveCoroutine);
        }
        else animator?.SetTrigger("pain");
    }

    protected IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, boidAwarenessRadius);

        Gizmos.color = Color.blue;
        foreach (Transform t in nearbyEnemies)
            Gizmos.DrawLine(transform.position, t.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + movementLine);
    }
}
