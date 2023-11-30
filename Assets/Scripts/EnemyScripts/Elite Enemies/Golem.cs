using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    //This enemy has a fuckton going on
    //Honestly he's my boi
    [SerializeField] LayerMask targetLayer;

    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] public float aggroDistance = 10f;
    [SerializeField] private float attackDistance = 1f;

    [SerializeField, Range(0, 14)] private int attackDmgStomp = 3;
    [SerializeField, Range(0, 14)] private int attackDmgRanged = 3;
    [SerializeField] private float knockbackStomp = 8f;

    [SerializeField] private float attackStartup = 1f;
    private float nextDamageTimeMelee = 0;
    private float nextDamageTimeRanged = 0;

    [SerializeField] private float damageIntervalMelee = 1f; // in seconds
    [SerializeField] private float damageIntervalRange = 1f; // in seconds

    private Vector3 direction;

    private float distanceFromTarget = 999f;

    private bool inAttackRange = false;
    private bool inAggroRange = false;

    private bool isAttacking = false;

    private bool canMeleeAttack = false;
    private bool canRangedAttack = false;
    private bool canMove = false;

    private Coroutine disableMoveCoroutine;

    private Coroutine colorChangeCoroutine;
    [SerializeField] public float colorSpeed = 1f;
    public Color regularColor = Color.white;
    public Color hurtColor = Color.red;

    private SpriteRenderer spriteRenderer;

    [SerializeField] float attackDelayStomp;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] float attackImpact;
    [SerializeField] GameObject vfxObj;

    //============================================================
    [SerializeField] bool isActiveAtStart = false;

    [SerializeField] float activationRange;
    [SerializeField] float activationTime;
    bool isActive = false;
    bool inActiveRange = false;

    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] float projectileStartup = 1f;
    [SerializeField] float projectileSpeed = 1f;
    [SerializeField] private float projectileDuration;
    bool inMeleeRange;
    [SerializeField] float meleeRange = 3f;
    protected GameObject previousTarget = null;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = gfxObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromTarget = targetDistance();
        inActiveRange = distanceFromTarget <= activationRange;

        if(inActiveRange && !isActive)
            StartCoroutine(ActivateGolem());

        if (isActiveAtStart)
            isActive = true;

        direction = currentTarget.transform.position - transform.position;

        //set this object to look at the player at any given point in time on the horizontal plane
        transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

        //don't do shit if mid-attack
        //The golem also does jack diddly fuckin squat if not active. He's eepy
        if (isAttacking || !isActive)
            return;

        inAggroRange = distanceFromTarget <= aggroDistance;
        inAttackRange = distanceFromTarget <= attackDistance;
        inMeleeRange = distanceFromTarget <= meleeRange;

        if(canMove)
            animator.SetFloat("walkSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));
        else
            animator.SetFloat("walkSpeed", 0);

        if (nextDamageTimeMelee <= Time.time && !canMeleeAttack)
            canMeleeAttack = true;

        if (nextDamageTimeRanged <= Time.time && !canRangedAttack)
            canRangedAttack = true;

        if (currentTarget != null && !isDead)
        {
            // Store the previous target before updating
            previousTarget = currentTarget;

            // Update the target based on proximity
            UpdateTarget();

            // If the target has changed, print the new target
            if (previousTarget != currentTarget)
            {
                Debug.Log("Golem switched to: " + currentTarget.name);
            }

            if (canMeleeAttack && inMeleeRange)
            {
                isAttacking = true;
                StartCoroutine(Stomp());
            }
            else if (canRangedAttack && inAttackRange)
            {
                isAttacking = true;
                StartCoroutine(RangedAttack());
            }
            else if (inAggroRange && canMove)
            {
                moveTowardsTarget();
            }
        }
    }

    private IEnumerator Stomp()
    {
        isAttacking = true;
        DisableAttackVFX();

        animator.SetTrigger("attackStart");
        yield return new WaitForSeconds(attackStartup);

        animator.SetTrigger("stomp");
        yield return new WaitForSeconds(attackDelayStomp);

        if (isDead)
        {
            ResetAttackMelee();
            yield break;
        }

        CameraShake.instance.ShakeCamera(impulseSource);
        PlayAttackVFX();
        AudioManager.instance.PlayAll(new string[] { "damage_2", "impact_4", "rumble_impact" });
        StompAttack();

        StartCoroutine(DisableMovementForSeconds(damageIntervalMelee));
        ResetAttackMelee();
    }

    private IEnumerator RangedAttack()
    {
        animator.SetTrigger("attackRangedStart");
        yield return new WaitForSeconds(projectileStartup);

        if (isDead)
        {
            ResetAttackRanged();
            yield break;
        }

        // Create a new instance of the projectile using Instantiate
        GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(direction));

        // Get the Projectile component from the new projectile if it has one
        ProjectileProperties projectile = newProjectile.GetComponent<ProjectileProperties>();

        // Check if the projectile has a Projectile component
        if (projectile != null)
        {
            // Set the properties of the newly instantiated projectile
            projectile.InitializeProjectile(projectileSpeed, projectileDuration, attackDmgRanged, 1f, transform.forward);
        }

        ResetAttackRanged();
    }

    private void StompAttack()
    {
        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);

        rb.AddForce(attackImpact * direction.normalized, ForceMode.Impulse);
        hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);
        foreach (Collider collider in hitTarget)
            base.DealDamage(attackDmgStomp, knockbackStomp, collider.gameObject);
    }

    private IEnumerator ActivateGolem()
    {
        AudioManager.instance.PlayAll(new string[] { "golem_bootup_1", "golem_bootup_2" });
        animator.SetTrigger("activate");
        isActive = true;
        isAttacking = true;
        yield return new WaitForSeconds(activationTime);
        isAttacking = false;
        canMeleeAttack = true;
        canRangedAttack = true;
        canMove = true;
    }

    private void moveTowardsTarget()
    {
        // Change animator to walk01
        Vector3 direction = currentTarget.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.velocity = new Vector3(horizontalDirection.x * moveSpeed, rb.velocity.y, horizontalDirection.z * moveSpeed);
    }

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
        }
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

    private IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    private void ResetAttackMelee()
    {
        //add some randomness to the next time he'll attack
        nextDamageTimeMelee = Time.time + damageIntervalMelee;
        nextDamageTimeRanged = Time.time + damageIntervalRange;
        canRangedAttack = false;
        canMeleeAttack = false;
        isAttacking = false;
    }

    private void ResetAttackRanged()
    {
        //add some randomness to the next time he'll attack
        nextDamageTimeRanged = Time.time + damageIntervalRange;
        canRangedAttack = false;
        isAttacking = false;
    }

    private void PlayAttackVFX()
    {
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        vfxObj.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackSize);
    }
}
