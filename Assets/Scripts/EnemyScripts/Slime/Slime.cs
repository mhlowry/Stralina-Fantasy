using System.Collections;
using UnityEngine;

public class Slime : Enemy
{
    public float moveSpeed = 5f;
    public float colorSpeed = 0.5f;
    public Color regularColor = Color.white;
    public Color hurtColor = Color.red;
    private Material material;
    private Coroutine colorChangeCoroutine;
    [SerializeField] public float aggroDistance = 10f;
    [SerializeField] public float attackDistance = 2f;
    [SerializeField] public float colliderCheck = 2f;
    [SerializeField] public float attackPower = 1f;
    [SerializeField] public float knockback = 1f;
    private bool canMove = true;
    private float distanceFromPlayer = 999f;
    private float nextDamageTime = 0;
    private float damageInterval = 1f; // in seconds
    private bool canAttack = true;


    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
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
        
        // if player exists, is within aggro distance, and move isn't on cooldown
        if (playerObject != null && distanceFromPlayer <= aggroDistance && canMove)
        {
            Vector3 direction = playerObject.transform.position - transform.position;
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

            // if within attack distance, attack
            if (distanceFromPlayer <= attackDistance && canAttack)
            {
                DealDamage(attackPower, knockback, direction);
                nextDamageTime = Time.time + damageInterval;
            }
            // else, move towards player
            else 
                rb.velocity = new Vector3(horizontalDirection.x * moveSpeed, rb.velocity.y, horizontalDirection.z * moveSpeed);
        }
    }

     public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //change color
        material.color = hurtColor;

        //start the color change coroutine to return to base color
        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);

        colorChangeCoroutine = StartCoroutine(ChangeColor());

        //inflict damage
        base.TakeDamage(damage, knockback, direction);
        Debug.Log(gameObject.name + ": \"Ouch!  My current Hp is only " + curHealthPoints + "!\"");

        StartCoroutine(DisableMovementForSeconds(0.5f));

        //die if dead
        if(curHealthPoints <= 0)
        {
            Debug.Log(gameObject.name + " Fucking Died");
            base.Die();
        }
    }

    private IEnumerator ChangeColor()
    {
        //this changes the color, duh
        float tick = 0f;
        while (material.color != regularColor)
        {
            tick += Time.deltaTime * colorSpeed;
            material.color = Color.Lerp(hurtColor, regularColor, tick);
            yield return null;
        }
    }

    private IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }
}
