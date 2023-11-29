using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicKing : Enemy
{
    [SerializeField] LayerMask targetLayer;

    private Vector3 spawnPoint;
    private Vector3 targetPosition;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float timeBetweenNewPosition = 2f;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] public float aggroDistance = 10f;

    //MELEE__________________________________________________________________________
    [SerializeField, Range(0, 14)] protected int attackDmgMelee = 3;
    [SerializeField] protected float knockbackVert = 7f;

    [SerializeField] protected float attackStartupMelee = 1f;
    [SerializeField] float attackDelayMelee;
    bool inMeleeRange;
    [SerializeField] float meleeRange = 3f;

    [SerializeField] List<Transform> attackPointsMelee;
    [SerializeField] float attackSizeMelee;
    [SerializeField] GameObject vfxObj;

    //RANGED_______________________________________________________________________________________
    [SerializeField, Range(0, 14)] protected int attackDmgRanged = 3;
    [SerializeField] protected float knockbackRanged = 7f;

    [SerializeField] float attackStartupRanged = 1f;
    [SerializeField] float attackDelayRanged;
    bool inRangedAtkRange;
    [SerializeField] float rangedAttackRange = 50f;

    [SerializeField] Transform projectileSpawn;
    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] GameObject vfxObj;

    [SerializeField] float projectileSpeed = 1f;
    [SerializeField] private float projectileDuration;

    //Spawning Shit___________________________________________________________________________________
    [SerializeField] GameObject mimic;
    [SerializeField] List<Transform> spawnpoints;
    [SerializeField] float spawnEnemiesStartup = 1f;
    [SerializeField] float spawnDelay;

    private bool canSpawnMimics = true;
    [SerializeField] protected float spawnInterval = 1f; // in seconds
    float nextSpawnTime;

    //MISC__________________________________________________________________________
    protected Vector3 direction;

    protected float distanceFromTarget = 999f;

    protected bool hitMidAttack = false;

    protected bool inAggroRange = false;

    protected bool isAttacking = false;

    [SerializeField] protected float actionInterval = 1f; // in seconds
    float nextActionTime;
    private bool canAct = true;
    protected bool canMove = true;

    private Coroutine colorChangeCoroutine;
    [SerializeField] public float colorSpeed = 1f;
    public Color regularColor = Color.white;
    public Color hurtColor = Color.red;

    private Coroutine disableMoveCoroutine;

    private SpriteRenderer spriteRenderer;
    protected GameObject previousTarget = null;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = gfxObject.GetComponent<SpriteRenderer>();
        gameObject.layer = LayerMask.NameToLayer("Skeleton");
    }

    private void Start()
    {
        StartCoroutine(UpdateTargetPositionCoroutine());
        spawnPoint = transform.position;
    }

    void Update()
    {
        distanceFromTarget = targetDistance();
        direction = currentTarget.transform.position - transform.position;

        //set this object to look at the player at any given point in time on the horizontal plane
        transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

        //don't do shit if mid-attack
        if (isAttacking)
            return;

        inAggroRange = distanceFromTarget <= aggroDistance;
        inRangedAtkRange = distanceFromTarget <= rangedAttackRange;
        inMeleeRange = distanceFromTarget <= meleeRange;

        if (canMove)
            animator.SetFloat("walkSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));
        else
            animator.SetFloat("walkSpeed", 0);

        if (nextActionTime <= Time.time && !canAct)
            canAct = true;

        if (nextSpawnTime <= Time.time && !canSpawnMimics)
            canSpawnMimics = true;

        if (currentTarget != null && inAggroRange && !isDead)
        {
            // Store the previous target before updating
            previousTarget = currentTarget;

            // Update the target based on proximity
            UpdateTarget();

            // If the target has changed, print the new target
            if (previousTarget != currentTarget)
            {
                Debug.Log("King switched to: " + currentTarget.name);
            }

            if (canAct && inMeleeRange)
            {
                isAttacking = true;
                StartCoroutine(StartMeleeAttack());
            }
            else if (canAct && inRangedAtkRange)
            {
                isAttacking = true;
                if(canSpawnMimics)
                    StartCoroutine(SpawnEnemies());
                else
                    StartCoroutine(RangedAttack());
            }
            else if (canMove)
                WanderAimlessly();
        }
    }

    private void WanderAimlessly()
    {
        if (isDead || !canMove)
            return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Keep the y-axis movement to 0 to ensure sliding movement
        rb.velocity = direction * moveSpeed * Time.deltaTime;
    }

    //MELEE ATTACK STUFF=====================================================================================
    private IEnumerator StartMeleeAttack()
    {
        DisableAttackVFX();

        animator.SetTrigger("meleeAttackStart");
        yield return new WaitForSeconds(attackStartupMelee);

        animator.SetTrigger("meleeAttackActivate");
        yield return new WaitForSeconds(attackDelayMelee);
        AudioManager.instance.PlayAll(new string[] { "swish_1_deep", "projectile_woosh_1", "sword_1" });

        if (isDead)
        {
            ResetAction();
            yield break;
        }

        PlayAttackVFX();
        //AudioManager.instance.PlayAll(new string[] { "damage_2", "impact_4", "rumble_impact" });
        MeleeAttack();

        StartCoroutine(DisableMovementForSeconds(2f));
        ResetAction();

        if(canSpawnMimics)
            StartCoroutine(SpawnEnemies());
    }

    private IEnumerator RangedAttack()
    {
        animator.SetTrigger("rangedAttackStart");
        AudioManager.instance.Play("fireball_ambience_1");
        yield return new WaitForSeconds(attackStartupRanged);

        animator.SetTrigger("rangedAttackActivate");
        yield return new WaitForSeconds(attackDelayRanged);

        if (isDead)
        {
            ResetAction();
            yield break;
        }

        AudioManager.instance.Stop("fireball_ambience_1");

        distanceFromTarget = targetDistance();
        float yDisplacement = (projectileSpawn.position.y - currentTarget.transform.position.y) - 1;
        float duration = yDisplacement / projectileSpeed;

        // Create a new instance of the projectile using Instantiate
        GameObject newProjectile = GameObject.Instantiate(projectilePrefab, projectileSpawn.position, Quaternion.LookRotation(direction));

        //jesus christ I can't beliieve this got turned into a physics assignment, AGAIN
        float xVelVector = (currentTarget.transform.position.x - projectileSpawn.position.x) / duration;
        float yVelVector = 0; //FUCK YEAH
        float zVelVector = (currentTarget.transform.position.z - projectileSpawn.position.z) / duration;

        Vector3 launchDirection = new Vector3(xVelVector, yVelVector, zVelVector);

        // Get the Projectile component from the new projectile if it has one
        ProjectileProperties projectile = newProjectile.GetComponent<ProjectileProperties>();

        // Check if the projectile has a Projectile component
        if (projectile != null)
        {
            // Set the properties of the newly instantiated projectile
            //make projectile speed 1f because our force comes from the valuse in launchDirection.  Using gravity-based projectile is compllicated so this is good practice
            projectile.InitializeProjectile(projectileDuration, attackDmgRanged, 1f, launchDirection);
        }

        StartCoroutine(DisableMovementForSeconds(2f));
        ResetAction();
    }

    private void MeleeAttack()
    {
        List<Collider[]> hitTarget = new List<Collider[]>();

        foreach (Transform hitBox in attackPointsMelee)
        {
            hitTarget.Add(Physics.OverlapSphere(hitBox.position, attackSizeMelee, targetLayer));
        }

        foreach (Collider[] targetList in hitTarget)
        {
            foreach (Collider c in targetList)
            {
                base.DealDamage(attackDmgMelee, knockbackVert, c.gameObject);
                //return; //pnly damage player once
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        ResetSpawner();
        yield return new WaitForSeconds(0.5f);

        AudioManager.instance.Play("king_roar");
        animator.SetTrigger("spawnStart");
        yield return new WaitForSeconds(spawnEnemiesStartup);

        foreach(Transform t in spawnpoints)
        {
            Instantiate(mimic, t.position, Quaternion.LookRotation(Vector3.zero));
        }

        yield return new WaitForSeconds(spawnDelay);
        animator.SetTrigger("spawnEnd");

        if (canAct)
            StartCoroutine(RangedAttack());
    }

    //MISC==============================================================================================
    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        spriteRenderer.color = hurtColor;

        //start the color change so it doesn't start mid combo
        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);

        colorChangeCoroutine = StartCoroutine(ChangeColor());

        //start the disablemove so it doesn't start mid combo
        if (disableMoveCoroutine != null)
            StopCoroutine(disableMoveCoroutine);

        disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(0.5f));

        //inflict damage
        base.TakeDamage(damage, knockback, direction);

        //die if dead
        //make sure they're not already dying, prevent from calling "die" twice
        if (curHealthPoints <= 0 && !animator.GetBool("isDead"))
        {
            //Debug.Log(gameObject.name + " Fucking Died");
            canMove = false;
            Die();
            StopCoroutine(colorChangeCoroutine);
            StopCoroutine(disableMoveCoroutine);
        }
    }

    protected IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    private void ResetAction()
    {
        //add some randomness to the next time he'll attack
        nextActionTime = Time.time + actionInterval;
        canAct = false;
        isAttacking = false;
    }

    private void ResetSpawner()
    {
        //add some randomness to the next time he'll attack
        nextSpawnTime = Time.time + spawnInterval;
        canSpawnMimics = false;
    }

    private void PlayAttackVFX()
    {
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        vfxObj.SetActive(false);
    }

    private IEnumerator ChangeColor()
    {
        //this changes the color, duh
        float tick = 0f;
        while (spriteRenderer.color != regularColor)
        {
            tick += Time.deltaTime * colorSpeed;
            spriteRenderer.color = Color.Lerp(hurtColor, regularColor, tick);
            yield return null;
        }
    }

    private IEnumerator UpdateTargetPositionCoroutine()
    {
        while (true)
        {
            // Update target position to a new random position within the bounds of the spawnpoint
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += spawnPoint;
            randomDirection.y = transform.position.y; // Assuming you want the companion to stay at its current y position

            targetPosition = randomDirection;

            // Wait for some time before changing the target position again
            yield return new WaitForSeconds(timeBetweenNewPosition);
        }
    }

    void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        foreach (Transform t in attackPointsMelee)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, attackSizeMelee);
        }
    }
}
