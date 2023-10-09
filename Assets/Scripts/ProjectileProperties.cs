using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ProjectileProperties : MonoBehaviour
{
    private float speed = 20f;
    private float duration = 10f;
    private int damage;
    private float knockback;
    private UnityEngine.Vector3 direction;

    private float initialTime;

    [SerializeField] private LayerMask targetMask;

    private HashSet<Collider> loggedEnemies = new HashSet<Collider>();

    private Rigidbody rb;

    public void InitializeProjectile(float speed, float duration, int damage, float knockback, UnityEngine.Vector3 direction)
    {
        this.speed = speed;
        this.duration = duration;
        this.damage = damage;
        this.knockback = knockback;
        this.direction = direction;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        initialTime = Time.time;
        rb.velocity = transform.forward * speed;
    }

    private void Update()
    {
        if(Time.time - initialTime > duration)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider hitTarget)
    {
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
