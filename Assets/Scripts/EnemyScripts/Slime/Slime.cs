using System.Collections;
using UnityEngine;

public class Slime : Enemy
{
    public float moveSpeed = 5f;
    public float colorSpeed = 0.5f;
    private Coroutine disableMoveCoroutine;
    [SerializeField] public float aggroDistance = 10f;
    [SerializeField] public float attackDistance = 1f;
    [SerializeField] public float attackPower = 1f;
    [SerializeField] public float knockback = 1f;
    private bool canMove = true;
    private float distanceFromPlayer = 999f;
    private float nextDamageTime = 0;
    private float damageInterval = 1f; // in seconds
    private bool canAttack = true;
    private bool inAttackRange = false;
    private bool inAggroRange = false;

    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        SlideTowardsPlayer();

        if (nextDamageTime <= Time.time)
            canAttack = true;
    }

    private void SlideTowardsPlayer()
    {
        distanceFromPlayer = playerDistance();
        inAggroRange = distanceFromPlayer <= aggroDistance;
        inAttackRange = distanceFromPlayer <= attackDistance;
        
        // if player exists, is within aggro distance, and move isn't on cooldown
        if (playerObject != null && inAggroRange && canMove)
        {
            animator.SetBool("isAggro", true);
            Vector3 direction = playerObject.transform.position - transform.position;
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

            // if within attack distance, attack
            if (inAttackRange)
            {
                animator.SetBool("isAttack", true);
                if (canAttack)
                {
                    DealDamage(attackPower, knockback, direction);
                    nextDamageTime = Time.time + damageInterval;
                }
            }
            // else, move towards player
            else
            {
                animator.SetBool("isAttack", false);
                rb.velocity = new Vector3(horizontalDirection.x * moveSpeed, rb.velocity.y, horizontalDirection.z * moveSpeed);
            }
        }
        else
        {
            animator.SetBool("isAggro", false);
            animator.SetBool("isAttack", false);
        }
    }

     public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //start the disablemovement so it doesn't start mid combo
        if (disableMoveCoroutine != null)
            StopCoroutine(disableMoveCoroutine);

        disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(0.8f));

        //inflict damage
        base.TakeDamage(damage, knockback, direction);
        Debug.Log(gameObject.name + ": \"Ouch!  My current Hp is only " + curHealthPoints + "!\"");

        animator.SetTrigger("pain");

        //die if dead
        //make sure they're not already dying, prevent from calling "die" twice
        if(curHealthPoints <= 0 && !animator.GetBool("isDead"))
        {
            Debug.Log(gameObject.name + " Fucking Died");
            canMove = false;
            base.Die();
            StopCoroutine(disableMoveCoroutine);
        }
        else animator.SetTrigger("isHit");
    }

    private IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }
}
