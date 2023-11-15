using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonParent : FootSoldier
{
    [SerializeField] protected float timeCoreExposed = 5f;
    [SerializeField] protected float timeToRegenerate = 0.35f;

    [SerializeField] protected int maxCoreHealth = 50;
    protected int curCoreHealth;

    protected bool inCoreState = false;
    protected bool isRegenerating = false;

    protected override void Awake()
    {
        base.Awake();
        gameObject.layer = LayerMask.NameToLayer("Skeleton");
    }

    private void Start()
    {
        curCoreHealth = maxCoreHealth;
    }

    protected override void Update()
    {
        animator.SetBool("coreExposed", inCoreState);
        base.Update();
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //don't take damage if you're dead, duh
        if (isDead || isRegenerating)
            return;

        //inflict damage to Skeleton if not in core state
        if (!inCoreState)
        {
            //start the disablemove so it doesn't start mid combo
            if (disableMoveCoroutine != null)
                StopCoroutine(disableMoveCoroutine);

            disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(2f));

            if (isAttacking)
                hitMidAttack = true;

            base.TakeDamage(damage, knockback, direction);

            //Enter the core state if dead
            //make sure they're not already dying, prevent from calling the falling apart animation
            if (curHealthPoints <= 0 && !animator.GetBool("coreExposed"))
            {
                canMove = false;
                StopCoroutine(disableMoveCoroutine);
                StartCoroutine(CoreTimer());
            }
            else animator?.SetTrigger("pain");
        }
        else //otherwise, inflict damage to the core.
        {
            TakeCoreDamage(damage);
        }
    }

    protected void TakeCoreDamage(int damage)
    {
        //add screenshake on impact
        CameraShake.instance.ShakeCamera(impulseSource);

        AudioManager.instance.PlayRandom(new string[] { "impact_1", "impact_2", "impact_3", "impact_4", "impact_5", "impact_6" });
        HitStop.instance.Stop(0.02f); //0.03f

        curCoreHealth -= damage;
        if (curCoreHealth <= 0 && !animator.GetBool("isDead"))
        {
            canMove = false;
            Die();
            StopCoroutine(disableMoveCoroutine);
        }
        else animator?.SetTrigger("pain");
    }

    private IEnumerator CoreTimer()
    {
        inCoreState = true;
        canAttack = false;
        animator.SetTrigger("enterCore");
        yield return new WaitForSeconds(timeCoreExposed);

        if (isDead)
        {
            yield break;
        }

        animator.SetTrigger("enterRegen");
        isRegenerating = true;
        yield return new WaitForSeconds(timeToRegenerate);

        inCoreState = false;
        isRegenerating = false;
        canMove = true;
        canAttack = true;
        curHealthPoints = maxHealthPoints;
    }
}
