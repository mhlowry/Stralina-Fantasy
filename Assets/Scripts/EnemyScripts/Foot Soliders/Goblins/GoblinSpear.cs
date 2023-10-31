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
    [SerializeField] float chargingDistance;
    bool inChargingRange;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        inChargingRange = distanceFromPlayer <= chargingDistance;
        SpearChoice();
    }

    private void SpearChoice()
    {
        //don't do shit if mid-attack
        if (isAttacking)
            return;

        if (playerObject != null && inAggroRange && canMove && !isDead)
        {
            if (canAttack && (inAttackRange || inChargingRange))
            {
                //Do a basic stab if the player is already wicked close
                if (inAttackRange)
                {
                    isAttacking = true;
                    StartCoroutine(BasicStrike());
                }
            }
            else
                base.NavigateCombat();
        }
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
        StabAttack();

        ResetAttack();
    }

    protected void PlayAttackVFX(Vector3 direction, GameObject vfxObj)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(direction);
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        regStabVfxObj.SetActive(false);
    }

    private void StabAttack()
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
                base.DealDamage(attackPower, knockback);
                return; //pnly damage player once
            }
        }
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
        foreach (Transform t in attackPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, attackSize);
        }
    }
}
