using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDagger : FootSoldier
{
    [SerializeField] float secondAttackStartup;

    [SerializeField] float attackdelay1;
    [SerializeField] float attackdelay2;

    [SerializeField] float attackImpact;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] GameObject vfxObj;
    [SerializeField] LayerMask playerLayer;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        DaggerChoice();
    }

    private void DaggerChoice()
    {
        //don't do shit if mid-attack
        if (isAttacking)
            return;

        if (playerObject != null && inAggroRange && canMove && !isDead)
        {
            if (canAttack && inAttackRange)
            {
                isAttacking = true;
                StartCoroutine(DaggerStrikes());
            }
            else
                base.NavigateCombat();
        }
    }

    private IEnumerator DaggerStrikes()
    {
        animator.SetTrigger("attackStart");
        yield return new WaitForSeconds(attackStartup);

        //play audio and animation
        animator.SetTrigger("slash_1");
        yield return new WaitForSeconds(attackdelay1);

        if (hitMidAttack || isDead)
        {
            hitMidAttack = false;
            ResetAttack();
            yield break;
        }
        AudioManager.instance.Play("sword_2");

        //do melee attack boilerplate
        DaggerAttack();

        yield return new WaitForSeconds(secondAttackStartup);

        //play audio and animation
        animator.SetTrigger("slash_2");
        yield return new WaitForSeconds(attackdelay2);

        if (hitMidAttack || isDead)
        {
            hitMidAttack = false;
            ResetAttack();
            yield break;
        }
        AudioManager.instance.Play("sword_3");

        //do melee attack boilerplate
        DaggerAttack();

        ResetAttack();
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

    protected void PlayAttackVFX(Vector3 direction)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(direction);
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        vfxObj.SetActive(false);
    }

    private void DaggerAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(attackPoint.position, attackSize, playerLayer);

        DisableAttackVFX();
        PlayAttackVFX(direction.normalized);
        rb.AddForce(attackImpact * direction.normalized, ForceMode.Impulse);
        hitPlayer = Physics.OverlapSphere(attackPoint.position, attackSize, playerLayer);
        foreach (Collider collider in hitPlayer)
            base.DealDamage(attackPower, knockback);
    }

    private void ResetAttack()
    {
        //add some randomness to the next time he'll attack
        nextDamageTime = Time.time + damageInterval + Random.Range(damageIntRandomMin, damageIntRandomMax);
        canAttack = false;
        isAttacking = false;
    }
}
