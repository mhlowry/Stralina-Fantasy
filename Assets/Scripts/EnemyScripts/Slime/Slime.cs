using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Slime : Enemy
{
    public float moveSpeed = 1f;
    public float damageSpeed = 2f;
    private Coroutine disableMoveCoroutine;
    [SerializeField] protected float aggroDistance = 10f;
    [SerializeField] protected float attackDistance = 1f;
    [SerializeField] protected float attackPower = 1f;
    [SerializeField] private float knockback = 1f;
    protected bool canMove = true;
    protected float distanceFromPlayer = 999f;
    protected float nextDamageTime = 0;
    [SerializeField] protected float damageInterval = 1f; // in seconds
    protected bool canAttack = true;
    protected bool inAttackRange = false;
    protected bool inAggroRange = false;

    protected Vector3 direction;

    [SerializeField] protected float moveStartup = 1f;
    [SerializeField] protected float damageStartup = 1f;
    [SerializeField] private float moveHeight = 4f;
    [SerializeField] private float damageHeight = 6f;

    protected bool isMoving = false;
    protected bool isAttacking = false;
    [SerializeField] protected float moveInterval = 1f;

    private bool enableDamage = false;

    protected override void Awake()
    {
        base.Awake();
    }

    void FixedUpdate()
    {
        //SlideTowardsPlayer();

        if (!isMoving)
            JumpTowardsPlayer();

        if (nextDamageTime <= Time.time && !canAttack)
            canAttack = true;

        animator?.SetFloat("vertVelocity", rb.velocity.y);

        animator?.SetBool("isLanded", Mathf.Abs(rb.velocity.y) <= 0.01f);
        
        //turns off damage on the collider when idle
        if(enableDamage && animator.GetCurrentAnimatorStateInfo(0).IsName("SlimeIdle"))
        {
            enableDamage = false;
        }
    }

    private void Update()
    {
        animator?.SetBool("isJumping", isMoving);
        animator?.SetBool("isAttack", isAttacking);
    }

    private void JumpTowardsPlayer()
    {
        distanceFromPlayer = playerDistance();
        inAggroRange = distanceFromPlayer <= aggroDistance;
        inAttackRange = distanceFromPlayer <= attackDistance;

        if (playerObject != null && inAggroRange && canMove)
        {
            if (inAttackRange && canAttack)
            {
                isAttacking = true;
                StartCoroutine(StartMove(damageStartup, moveInterval + 1f));
            }
            else
            {
                StartCoroutine(StartMove(moveStartup, moveInterval));
            }
        }
    }

    protected IEnumerator StartMove(float startup, float endlag)
    {
        Vector3 jumpForce; //not the shonen kind
        isMoving = true;
        yield return new WaitForSeconds(startup); //prep time before jump

        //I have to do this math and shit in the coroutine because otherwise it gets the player's direction wayyyyy too early
        direction = playerObject.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

        //more force behind jump when attacking
        if (isAttacking)
        {
            enableDamage = true;
            jumpForce = new Vector3(horizontalDirection.x * damageSpeed, damageHeight, horizontalDirection.z * damageSpeed);
        }
        else
            jumpForce = new Vector3(horizontalDirection.x * moveSpeed, moveHeight, horizontalDirection.z * moveSpeed);

        rb.velocity = jumpForce;
        //if not grounded, do not update the ability to do shit (this doesn't work and crashes the editor)
        //while (Mathf.Abs(rb.velocity.y) >= 0.001) { }

        if (disableMoveCoroutine != null)
            StopCoroutine(disableMoveCoroutine);

        disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(endlag));
        isMoving = false;
    }

    //This is how the slime deals damage!
    private void OnCollisionEnter(Collision hitTarget)
    {
        //return if not even attacking
        if (!enableDamage)
            return;

        if (hitTarget.gameObject.CompareTag("Player"))
        {
            //Debug.Log("CollisionEnter");
            base.DealDamage(attackPower, knockback, direction);
            enableDamage = false;
        }
    }

    private void OnCollisionStay(Collision hitTarget)
    {
        //return if not even attacking
        if (!enableDamage)
            return;

        if (hitTarget.gameObject.CompareTag("Player"))
        {
            //Debug.Log("CollisionEnter");
            base.DealDamage(attackPower, knockback, direction);
            enableDamage = false;
        }
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //start the disablemove so it doesn't start mid combo
        if (disableMoveCoroutine != null)
            StopCoroutine(disableMoveCoroutine);

        disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(1f));

        //inflict damage
        base.TakeDamage(damage, knockback, direction);
        //if hit in midair, can no longer harm the player
        enableDamage = false;
        //Debug.Log(gameObject.name + ": \"Ouch!  My current Hp is only " + curHealthPoints + "!\"");

        //die if dead
        //make sure they're not already dying, prevent from calling "die" twice
        if(curHealthPoints <= 0 && !animator.GetBool("isDead"))
        {
            Debug.Log(gameObject.name + " Fucking Died");
            canMove = false;
            base.Die();
            StopCoroutine(disableMoveCoroutine);
        }
        else animator?.SetTrigger("pain");
    }

    protected IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
        if (isAttacking)
        {
            nextDamageTime = Time.time + damageInterval;
            canAttack = false;
            isAttacking = false;
        }
    }
}
