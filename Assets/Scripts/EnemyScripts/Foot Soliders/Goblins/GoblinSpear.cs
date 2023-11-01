using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpear : FootSoldier
{
    [SerializeField] LayerMask playerLayer;

    [SerializeField] List<Transform> attackPoints;
    [SerializeField] float attackSize;
    [SerializeField] GameObject regStabVfxObj;
    [SerializeField] GameObject runningVfxObj;
    [SerializeField] GameObject dashStabVfxObj;

    [SerializeField] float chargingSpeedAdjustment;
    [SerializeField] int chargingDamage;
    [SerializeField] float chargingDistance;
    bool inChargingRange;
    bool isCharging;

    float defaultMoveSpeed;

    private void Start()
    {
        defaultMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        inChargingRange = distanceFromPlayer >= chargingDistance;
        SpearChoice();
    }

    private void SpearChoice()
    {
        //don't do shit if mid-attack
        if (isAttacking)
            return;

        //I know nested if statements are bad but honestly it's a student game I really don't think it's a big deal
        if (playerObject != null && inAggroRange && canMove && !isDead)
        {
            if (canAttack && (inAttackRange || inChargingRange) && !isCharging)
            {
                //Do a basic stab if the player is already wicked close
                if (inAttackRange)
                {
                    isAttacking = true;
                    StartCoroutine(BasicStrike());
                }
                else
                {
                    StartCoroutine(ChargingStrike());
                }
            }
            else
                base.NavigateCombat();
        }
    }

    private IEnumerator ChargingStrike()
    {
        isCharging = true;
        DisableAttackVFX();

        animator.SetBool("isCharging", true);
        animator.SetTrigger("attackStart");

        PlayAttackVFX(direction, runningVfxObj);
        moveSpeed = defaultMoveSpeed + chargingSpeedAdjustment;

        while(!inAttackRange)
            yield return null;

        yield return new WaitForSeconds(attackStartup);

        DisableAttackVFX();
        if (hitMidAttack || isDead)
        {
            ResetAttack();
            hitMidAttack = false;
            yield break;
        }

        animator.SetTrigger("stab");
        PlayAttackVFX(direction, dashStabVfxObj);
        StabAttack(chargingDamage);

        ResetAttack();
    }

    private IEnumerator BasicStrike()
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

        PlayAttackVFX(direction, regStabVfxObj);
        StabAttack(attackPower);

        ResetAttack();
    }

    protected void PlayAttackVFX(Vector3 direction, GameObject vfxObj)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(new Vector3 (direction.x, 0.0f, direction.z));
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        regStabVfxObj.SetActive(false);
        dashStabVfxObj.SetActive(false);
        runningVfxObj.SetActive(false);
    }

    private void StabAttack(int damage)
    {
        List<Collider[]> hitPlayer = new List<Collider[]>();

        foreach (Transform hitBox in attackPoints)
        {
            hitPlayer.Add(Physics.OverlapSphere(hitBox.position, attackSize, playerLayer));
        }

        foreach (Collider[] playerList in hitPlayer)
        {
            foreach (Collider c in playerList)
            {
                base.DealDamage(damage, knockback);
                return; //pnly damage player once
            }
        }
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        if (isCharging)
            hitMidAttack = true;

        base.TakeDamage(damage, knockback, direction);
    }

    private void ResetAttack()
    {
        //add some randomness to the next time he'll attack
        nextDamageTime = Time.time + damageInterval + Random.Range(damageIntRandomMin, damageIntRandomMax);
        canAttack = false;
        isAttacking = false;
        isCharging = false;
        animator.SetBool("isCharging", false);
        moveSpeed = defaultMoveSpeed;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        //Draw the hitbox
        foreach (Transform t in attackPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, attackSize);
        }
    }
}
