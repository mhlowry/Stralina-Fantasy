using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBow : SkeletonParent
{
    [SerializeField] LayerMask playerLayer;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] float attackImpact;
    [SerializeField] GameObject vfxObj;

    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] GameObject vfxObj;

    [SerializeField] float projectileStartup = 1f;
    [SerializeField] float projectileSpeed = 1f;
    [SerializeField] private float projectileDuration;
    bool inMeleeRange;
    [SerializeField] float meleeRange = 3f;

    // Start is called before the first frame update
    protected override void Update()
    {
        inMeleeRange = distanceFromPlayer <= meleeRange;
        base.Update();
        BowChoice();
    }

    private void BowChoice()
    {
        //don't do shit if mid-attack
        if (isAttacking)
            return;

        if (playerObject != null && canMove && !isDead)
        {
            if(canAttack && inMeleeRange)
            {
                isAttacking = true;
                StartCoroutine(Strike());
            }
            else if (canAttack && inAttackRange)
            {
                isAttacking = true;
                StartCoroutine(BowAttack());
            }
            else if(inAggroRange)
                base.NavigateCombat();
        }
    }

    private IEnumerator Strike()
    {
        isAttacking = true;
        DisableAttackVFX();

        animator.SetTrigger("attackStart");
        yield return new WaitForSeconds(attackStartup);

        if (hitMidAttack || isDead)
        {
            ResetAttack();
            hitMidAttack = false;
            yield break;
        }

        PlayAttackVFX(direction);
        BurstAttack();

        ResetAttack();
    }

    private IEnumerator BowAttack()
    {
        animator.SetTrigger("attackBow");

        yield return new WaitForSeconds(projectileStartup);

        if (hitMidAttack || isDead)
        {
            ResetAttack();
            hitMidAttack = false;
            yield break;
        }

        // Create a new instance of the projectile using Instantiate
        GameObject newProjectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.LookRotation(direction));

        // Get the Projectile component from the new projectile if it has one
        ProjectileProperties projectile = newProjectile.GetComponent<ProjectileProperties>();

        // Check if the projectile has a Projectile component
        if (projectile != null)
        {
            // Set the properties of the newly instantiated projectile
            projectile.InitializeProjectile(projectileSpeed, projectileDuration, attackPower, 1f, transform.forward);
        }

        ResetAttack();
    }

    protected void PlayAttackVFX(Vector3 direction)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(direction);
        vfxObj.SetActive(true);
    }

    private void BurstAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackSize, playerLayer);

        DisableAttackVFX();
        PlayAttackVFX(direction.normalized);
        rb.AddForce(attackImpact * direction.normalized, ForceMode.Impulse);
        hitPlayer = Physics.OverlapSphere(attackPoint.position, attackSize, playerLayer);
        foreach (Collider collider in hitPlayer)
            base.DealDamage(attackPower, knockback);
    }

    public virtual void DisableAttackVFX()
    {
        vfxObj.SetActive(false);
    }

    private void ResetAttack()
    {
        //add some randomness to the next time he'll attack
        nextDamageTime = Time.time + damageInterval + Random.Range(damageIntRandomMin, damageIntRandomMax);
        canAttack = false;
        isAttacking = false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        //Draw the hitbox
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackSize);
    }
}