using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalSlime : Slime
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] GameObject vfxObj;

    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        //SlideTowardsTarget();

        if (!isMoving)
            SlideTowardsTarget();

        if (nextDamageTime <= Time.time && !canAttack)
        {
            DisableAttackVFX();
            canAttack = true;
        }
    }

    void Update()
    {
        animator?.SetBool("isMoving", isMoving);
        animator?.SetBool("isAttack", isAttacking);
        direction = currentTarget.transform.position - transform.position;
    }

    private void SlideTowardsTarget()
    {
        if (isAttacking)
            return;

        distanceFromTarget = targetDistance();
        inAggroRange = distanceFromTarget <= aggroDistance;
        inAttackRange = distanceFromTarget <= attackDistance;

        // Store the previous target before updating
        previousTarget = currentTarget;

        // Update the target based on proximity
        UpdateTarget();

        // If the target has changed, print the new target
        if (previousTarget != currentTarget)
        {
            Debug.Log("Metal slime switched to: " + currentTarget.name);
        }

        if (currentTarget != null && inAggroRange && canMove)
        {

            if (inAttackRange && canAttack)
            {
                isAttacking = true;
                animator?.SetTrigger("AttackStart");
                StartCoroutine(MetalAttack());
            }
            else
            {
                StartCoroutine(StartMove(moveStartup, moveInterval));
            }
        }
    }

    private IEnumerator MetalAttack()
    {
        yield return new WaitForSeconds(damageStartup);

        if (hitMidAttack || isDead)
        {
            nextDamageTime = Time.time + damageInterval;
            canAttack = false;
            isAttacking = false;
            hitMidAttack = false;
            yield break;
        }

        PlayAttackVFX(direction);
        AudioManager.instance.Play("sword_1");
        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);
        foreach (Collider collider in hitTarget)
            base.DealDamage(attackPower, knockback);

        nextDamageTime = Time.time + damageInterval;
        canAttack = false;
        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        Gizmos.DrawWireSphere(attackPoint.position, attackSize);
    }

    protected void PlayAttackVFX(Vector3 direction)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        vfxObj.SetActive(false);
    }
}
