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

    private List<Enemy> enemiesInTrigger = new List<Enemy>();

    private void OnTriggerEnter(Collider hitTarget)
    {
        if (hitTarget.gameObject.CompareTag("Enemy"))
        {
            Enemy thisEnemy = hitTarget.GetComponent<Enemy>();
            if (thisEnemy != null && !enemiesInTrigger.Contains(thisEnemy))
            {
                enemiesInTrigger.Add(thisEnemy);
            }
        }
    }

    private void OnTriggerExit(Collider hitTarget)
    {
        if (hitTarget.gameObject.CompareTag("Enemy"))
        {
            Enemy thisEnemy = hitTarget.GetComponent<Enemy>();
            if (thisEnemy != null && enemiesInTrigger.Contains(thisEnemy))
            {
                enemiesInTrigger.Remove(thisEnemy);
            }
        }
    }

    private void Update()
    {
        if (Time.time - lastHitTime >= delayStrikes)
        {
            foreach (Enemy enemy in enemiesInTrigger)
            {
                if(enemy != null)
                    enemy.TakeDamage(damage, knockback, direction);
            }
            lastHitTime = Time.time;
        }
    }
}
