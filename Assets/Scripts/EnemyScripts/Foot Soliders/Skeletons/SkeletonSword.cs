using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSword : SkeletonParent
{
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] float attackImpact;
    [SerializeField] GameObject vfxObj;
    [SerializeField] LayerMask targetLayer;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        SwordChoice();
    }

    private void SwordChoice()
    {
        //don't do shit if mid-attack
        if (isAttacking)
            return;

        if (playerObject != null && inAggroRange && canMove && !isDead)
        {
            if (canAttack && inAttackRange)
            {
                isAttacking = true;
                StartCoroutine(Strike());
            }
            else
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
        SwordAttack();

        ResetAttack();
    }

    protected void PlayAttackVFX(Vector3 direction)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(direction);
        vfxObj.SetActive(true);
    }

    private void SwordAttack()
    {
        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);

        DisableAttackVFX();
        PlayAttackVFX(direction.normalized);
        rb.AddForce(attackImpact * direction.normalized, ForceMode.Impulse);
        hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);
        foreach (Collider collider in hitTarget)
            base.DealDamage(attackPower, knockback, collider.gameObject);
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
