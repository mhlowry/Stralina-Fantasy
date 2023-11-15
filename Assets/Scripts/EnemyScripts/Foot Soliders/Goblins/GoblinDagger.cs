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
    [SerializeField] LayerMask targetLayer;

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

        if (currentTarget != null && inAggroRange && canMove && !isDead)
        {
            // Store the previous target before updating
            previousTarget = currentTarget;

            // Update the target based on proximity
            UpdateTarget();

            // If the target has changed, print the new target
            if (previousTarget != currentTarget)
            {
                Debug.Log("Goblin (dagger) switched to: " + currentTarget.name);
            }

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

    private void DaggerAttack()
    {
        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);

        DisableAttackVFX();
        PlayAttackVFX(direction.normalized);
        rb.AddForce(attackImpact * direction.normalized, ForceMode.Impulse);
        hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);
        foreach (Collider collider in hitTarget)
            base.DealDamage(attackPower, knockback, collider.gameObject);
    }

    private void ResetAttack()
    {
        //add some randomness to the next time he'll attack
        nextDamageTime = Time.time + damageInterval + Random.Range(damageIntRandomMin, damageIntRandomMax);
        canAttack = false;
        isAttacking = false;
    }
}
