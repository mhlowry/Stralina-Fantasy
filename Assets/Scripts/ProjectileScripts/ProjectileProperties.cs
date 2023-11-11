using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ProjectileProperties : MonoBehaviour
{
    private float speed = 20f;
    private float duration = 10f;
    protected int damage = 1;
    [SerializeField] protected float knockback = 3f;
    protected UnityEngine.Vector3 direction;

    private float initialTime;

    [SerializeField] protected string targetMask;
    [SerializeField] protected string soundName;

    protected HashSet<Collider> loggedEnemies = new HashSet<Collider>();

    protected Rigidbody rb;

    public virtual void InitializeProjectile(float speed, float duration, int damage, float knockback, UnityEngine.Vector3 direction)
    {
        this.speed = speed;
        this.duration = duration;
        this.damage = damage;
        this.knockback = knockback;
        this.direction = direction.normalized;
    }

    //If you just want to use vector value rather than adding speed to a normalized vector
    public virtual void InitializeProjectile(float duration, int damage, float knockback, UnityEngine.Vector3 direction)
    {
        this.speed = 1f;
        this.duration = duration;
        this.damage = damage;
        this.knockback = knockback;
        this.direction = direction;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        initialTime = Time.time;
        rb.velocity = direction * speed;
        AudioManager.instance.Play(soundName);
    }

    protected virtual void Update()
    {
        if(Time.time - initialTime > duration)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider hitTarget)
    {
        GameObject targetObject = hitTarget.gameObject;

        //call takedamage for enemy if want to hit enemy
        if (targetMask == "Enemy" && hitTarget.gameObject.CompareTag("Enemy"))
        {
            Enemy thisEnemy = targetObject.GetComponent<Enemy>();
            if (!loggedEnemies.Contains(hitTarget))
            {
                //this is the main attack shit
                thisEnemy.TakeDamage(damage, knockback, direction);
                loggedEnemies.Add(hitTarget);
            }
        }
        else if(targetMask == "Player" && hitTarget.gameObject.CompareTag("Player"))
        {
            //Attack the player
            Player thisPlayer = targetObject.GetComponent<Player>();
            //this is the main attack shit
            thisPlayer.TakeDamage(damage, knockback, gameObject.transform.position);
        }
    }
}
