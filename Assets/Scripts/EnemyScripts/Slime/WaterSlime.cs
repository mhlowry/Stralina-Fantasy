using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSlime : Slime
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform explosionPoint;
    [SerializeField] float explosionSize;
    [SerializeField] float explosionDelay;
    [SerializeField] int explosionDamage;
    [SerializeField] float explosionKnockback;
    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] GameObject vfxObj;

    [SerializeField] float projectileSpeed = 1f;
    [SerializeField] private float projectileDuration;

    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        //SlideTowardsPlayer();

        if (!isMoving)
            SlideAwayFromPlayer();

        if (nextDamageTime <= Time.time && !canAttack)
        {
            //DisableAttackVFX();
            canAttack = true;
        }
    }

    void Update()
    {
        direction = (playerObject.transform.position - transform.position).normalized;

        animator?.SetBool("isMoving", isMoving);
        animator?.SetBool("isAttack", isAttacking);
        animator?.SetFloat("horizVelocity", (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)));
    }

    private void SlideAwayFromPlayer()
    {
        if (isAttacking || isDead)
            return;

        distanceFromPlayer = playerDistance();
        inAggroRange = distanceFromPlayer <= aggroDistance;
        inAttackRange = distanceFromPlayer <= attackDistance;

        if (playerObject != null && canMove)
        {
            if (inAggroRange)
            {
                AudioManager.instance.Play("water_dash");
                StartCoroutine(StartMove(moveStartup, moveInterval));
            }
            else if(inAttackRange && canAttack && !isMoving)
            {
                isAttacking = true;
                animator?.SetTrigger("AttackStart");
                StartCoroutine(WaterAttack());
            }
        }
    }

    private IEnumerator WaterAttack()
    {
        yield return new WaitForSeconds(damageStartup);
        //PlayAttackVFX(direction);

        if (hitMidAttack || isDead)
        {
            nextDamageTime = Time.time + damageInterval;
            canAttack = false;
            isAttacking = false;
            hitMidAttack = false;
            yield break;
        }

        distanceFromPlayer = playerDistance();
        float duration = distanceFromPlayer / projectileSpeed;

        // Create a new instance of the projectile using Instantiate
        GameObject newProjectile = GameObject.Instantiate(projectilePrefab, explosionPoint.position, Quaternion.LookRotation(direction));

        //jesus christ I can't beliieve this got turned into a physics assignment
        float xVelVector = (playerObject.transform.position.x - transform.position.x) / duration;
        float yVelVector = (0.5f * 9.8f * duration * duration) / duration;
        float zVelVector = (playerObject.transform.position.z - transform.position.z) / duration;

        //Debug.Log("Velx: " + xVelVector + "/ Vely: " + yVelVector + "/ Velz: " + zVelVector);

        Vector3 launchDirection = new Vector3(xVelVector, yVelVector, zVelVector);

        // Get the Projectile component from the new projectile if it has one
        ProjectileProperties projectile = newProjectile.GetComponent<ProjectileProperties>();

        // Check if the projectile has a Projectile component
        if (projectile != null)
        {
            // Set the properties of the newly instantiated projectile
            //make projectile speed 1f because our force comes from the valuse in launchDirection.  Using gravity-based projectile is compllicated so this is good practice
            projectile.InitializeProjectile(projectileDuration, attackPower, 1f, launchDirection);
        }

        nextDamageTime = Time.time + damageInterval;
        canAttack = false;
        isAttacking = false;
    }

    protected override void Die()
    {
        base.Die();
        StartCoroutine(ExplodeOnDeath());
    }

    private IEnumerator ExplodeOnDeath()
    {
        Debug.Log("Was called");
        yield return new WaitForSeconds(explosionDelay);
        AudioManager.instance.Play("water_splash");
        //eventually play a water explosion VFX
        //PlayAttackVFX(direction);

        Collider[] hitPlayer = Physics.OverlapSphere(explosionPoint.position, explosionSize, playerLayer);
        foreach (Collider collider in hitPlayer)
            collider.GetComponent<Player>().TakeDamage(explosionDamage, explosionKnockback, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        //Draw the explosion hitbox
        Gizmos.DrawWireSphere(explosionPoint.position, explosionSize);
    }

        /*    protected void PlayAttackVFX(Vector3 direction)
            {
                vfxObj.transform.rotation = Quaternion.LookRotation(direction);
                vfxObj.SetActive(true);
            }

            public virtual void DisableAttackVFX()
            {
                vfxObj.SetActive(false);
            }*/
    }
