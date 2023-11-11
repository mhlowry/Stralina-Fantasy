using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonShield : SkeletonSword
{
    bool canBlock = true;
    [SerializeField] int maxShieldHP;
    [SerializeField] int curShieldHP;
    [SerializeField] RuntimeAnimatorController regularController;

    protected override void Awake()
    {
        base.Awake();
        curShieldHP = maxShieldHP;
        gameObject.layer = LayerMask.NameToLayer("FootSoldier");
    }

    protected override void Update()
    {
        animator.SetBool("isBlocking", !isAttacking && canBlock);
        base.Update();
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //don't take damage if you're dead, duh
        if (isDead || isRegenerating)
            return;

        if (canBlock && !isAttacking)
        {
            HitStop.instance.Stop(0.04f);

            if (disableMoveCoroutine != null)
                StopCoroutine(disableMoveCoroutine);

            disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(0.5f));

            //Play blocking sounds
            AudioManager.instance.PlayAll(new string[] { "block_thud_1", "block_thud_3"});

            curShieldHP -= damage;
            if (curShieldHP <= 0 && !animator.GetBool("isDead"))
            {
                animator.SetTrigger("loseShield");
                canBlock = false;
                animator.runtimeAnimatorController = regularController;
            }
            else animator?.SetTrigger("pain");
        }
        else
        {
            base.TakeDamage(damage, knockback, direction);
        }
    }
}
