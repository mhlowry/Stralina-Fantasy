using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBreakOnImpact : ProjectileProperties
{
    //You gotta drag the gfx object and vfx object into these parts, respectively.
    //This is purely a basic ass projectile except it has some visual flair
    //Like, if a player object breaks on an enemy, it'll still hit enemies.  This is mostly a visual for enemies (golem, boss, etc)
    [SerializeField] GameObject vfxObj;
    [SerializeField] Animator animator;

    // Update is called once per frame
    protected override void OnTriggerEnter(Collider hitTarget)
    {
        base.OnTriggerEnter(hitTarget);
        if (targetMask == "Player" && hitTarget.gameObject.CompareTag("Player"))
        {
            vfxObj.SetActive(false);
            animator.SetTrigger("break");
            rb.velocity *= 0.1f;
        }
    }
}
