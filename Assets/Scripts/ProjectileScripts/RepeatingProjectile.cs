using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingProjectile : ProjectileProperties
{
    [SerializeField] private float delayStrikes = 0.5f;
    private float lastHitTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lastHitTime = Time.time;
    }

    // Update is called once per frame
    private void OnTriggerStay(Collider hitTarget)
    {
        if (Time.time - lastHitTime < delayStrikes)
            return;

        lastHitTime = Time.time;

        GameObject targetObject = hitTarget.gameObject;

        int targetMaskInt = (int)Mathf.Log(targetMask.value, 2);

        //Make sure we connected with the right target
        if (targetObject.layer != targetMaskInt)
            return;

        //call takedamage for enemy if want to hit enemy
        if (targetMaskInt == LayerMask.NameToLayer("Enemy"))
        {
            Enemy thisEnemy = targetObject.GetComponent<Enemy>();
            if (!loggedEnemies.Contains(hitTarget))
            {
                //this is the main attack shit
                thisEnemy.TakeDamage(damage, knockback, direction);
            }
        }
    }
}
