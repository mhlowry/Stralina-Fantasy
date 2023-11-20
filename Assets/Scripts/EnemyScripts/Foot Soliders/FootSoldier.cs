using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//FootSoldier is an enemy class that has universally cool movement and coordination..ish?
public class FootSoldier : Enemy
{
    protected Coroutine disableMoveCoroutine;

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
    [SerializeField] private float strafeRandomMin = -1.5f;
    [SerializeField] private float strafeRandomMax = 0.5f;

    private List<Transform> nearbyEnemies = new List<Transform>();

    private Vector3 movementLine;
    protected Vector3 direction;

    protected float distanceFromTarget = 999f;

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
        curStrafe = strafeDistance;
        changeStrafeInterval = Random.Range(2f, 4f);
    }

    protected virtual void Update()
    {
        distanceFromTarget = targetDistance();
        inAggroRange = distanceFromTarget <= aggroDistance;
        inAttackRange = distanceFromTarget <= attackDistance;

        direction = currentTarget.transform.position - transform.position;

        animator.SetFloat("walkSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));

        //set this object to look at the player at any given point in time on the horizontal plane
        transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

        //refresh enemies in range for seconds
        if (Time.time - timelastChecked > keepEnemiesForSeconds)
            FindNearbyEnemies();

        //change the strafe distance every so often just for kicks
        if (Time.time - lastStrafeChange > changeStrafeInterval)
        {
            lastStrafeChange = Time.time;
            curStrafe = strafeDistance + Random.Range(strafeRandomMin, strafeRandomMax);
        }

        if (nextDamageTime <= Time.time && !canAttack)
            canAttack = true;

        //Do not navigate combat in here.  do it in the children scripts.  See GoblinDagger or GoblinSpear for examples
        /*if (currentTarget != null && inAggroRange && canMove && !isDead)
        {
            NavigateCombat();
        }*/
    }

    protected GameObject previousTarget = null;

    //The goal, right, is to have a basic "smart" foot soldier that will strafe at a distance if they can't attack
    protected virtual void NavigateCombat()
    {
        Vector3 targetDirection = currentTarget.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        // Calculate a combined vector for nearby enemies
        Vector3 combinedDirection = horizontalDirection;

        //strafing state when foot soldier can't attack
        // If in strafe distance, avoid moving towards the target
        if (distanceFromTarget >= curStrafe - 0.5 && distanceFromTarget <= curStrafe + 0.5 && !canAttack)
        {
            // Do not move towards the target
            combinedDirection -= new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
        }

        if (distanceFromTarget < curStrafe && !canAttack)
        {
            // Reverse the direction when too close
            combinedDirection = -combinedDirection;
        }

        if (nearbyEnemies.Count > 0)
        {
            foreach (Transform t in nearbyEnemies)
            {
                if (t == null)
                {
                    continue;
                }
                Vector3 addToDirection = transform.position - t.position;
                Vector3 addHorizDirection = new Vector3(addToDirection.x, 0, addToDirection.z).normalized;
                combinedDirection += addHorizDirection;
            }
        }

        // Normalize the combined direction vector
        combinedDirection = combinedDirection.normalized;

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

    protected IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        try
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, boidAwarenessRadius);

            Gizmos.color = Color.blue;
            foreach (Transform t in nearbyEnemies)
                Gizmos.DrawLine(transform.position, t.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + movementLine);
        }
        catch 
        { }
    }
}
