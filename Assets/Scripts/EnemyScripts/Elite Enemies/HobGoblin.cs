using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HobGoblin : Enemy
{
    [SerializeField] LayerMask targetLayer;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] public float aggroDistance = 10f;
    [SerializeField] protected float attackDistance = 1f;

    [SerializeField, Range(0, 14)] protected int attackDmgSlash = 3;
    [SerializeField, Range(0, 14)] protected int attackDmgOverhead = 4;
    [SerializeField] protected float knockbackHor = 8f;
    [SerializeField] protected float knockbackVert = 10f;

    [SerializeField] protected float attackStartup = 1f;
    protected float nextDamageTime = 0;

    [SerializeField] protected float damageInterval = 1f; // in seconds

    protected Vector3 direction;

    protected float distanceFromTarget = 999f;

    protected bool hitMidAttack = false;

    protected bool inAttackRange = false;
    protected bool inAggroRange = false;

    protected bool isAttacking = false;

    protected bool canAttack = true;
    protected bool canMove = true;

    private Coroutine colorChangeCoroutine;
    [SerializeField] public float colorSpeed = 1f;
    public Color regularColor = Color.white;
    public Color hurtColor = Color.red;

    private Coroutine disableMoveCoroutine;

    private SpriteRenderer spriteRenderer;

    bool attackToggle = false;

    [SerializeField] float attackdelay1;
    [SerializeField] float attackdelay2;

    [SerializeField] List<Transform> attackPointsHor;
    [SerializeField] List<Transform> attackPointsVert;
    [SerializeField] float attackSizeHorz;
    [SerializeField] float attackSizeVert;

    [SerializeField] GameObject vfxHorizObj;
    [SerializeField] GameObject vfxVertObj;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = gfxObject.GetComponent<SpriteRenderer>();
        gameObject.layer = LayerMask.NameToLayer("FootSoldier");
    }

    // Update is called once per frame
    void Update()
    {
        direction = currentTarget.transform.position - transform.position;
        //set this object to look at the player at any given point in time on the horizontal plane
        transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));

        //don't do shit if mid-attack
        if (isAttacking)
            return;

        distanceFromTarget = targetDistance();
        inAggroRange = distanceFromTarget <= aggroDistance;
        inAttackRange = distanceFromTarget <= attackDistance;

        animator.SetFloat("walkSpeed", Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z));

        if (nextDamageTime <= Time.time && !canAttack)
            canAttack = true;

        if (currentTarget != null && inAggroRange && canMove && !isDead)
        {
            if (canAttack && inAttackRange)
            {
                isAttacking = true;
                StartCoroutine(BigFuckingSwing());
            }
            else
                moveTowardsTarget();
        }
    }

    private IEnumerator BigFuckingSwing()
    {

        DisableAttackVFX();

        attackToggle = !attackToggle;
        animator.SetBool("attackType", attackToggle);

        animator.SetTrigger("attackStart");
        //Debug.Log("what the fuck");
        yield return new WaitForSeconds(attackStartup);

        if (attackToggle)
        {
            //play audio and animation
            animator.SetTrigger("h_slash");
            yield return new WaitForSeconds(attackdelay1);
            if (isDead)
            {
                ResetAttack();
                yield break;
            }
            //AudioManager.instance.Play("sword_2");
            //do attack
            PlayAttackVFX(direction, vfxHorizObj);
            AttackHorz();
        }
        else
        {
            //just wait an extra half second for this one since it's more lethal
            yield return new WaitForSeconds(0.5f);
            //play audio and animation
            animator.SetTrigger("v_slash");
            yield return new WaitForSeconds(attackdelay2);
            if (isDead)
            {
                ResetAttack();
                yield break;
            }
            //AudioManager.instance.Play("sword_3");

            //do attack
            CameraShake.instance.ShakeCamera(impulseSource);
            PlayAttackVFX(direction, vfxVertObj);
            AttackVert();
        }

        StartCoroutine(DisableMovementForSeconds(damageInterval));
        ResetAttack();
    }

    private void AttackHorz()
    {
        List<Collider[]> hitTarget = new List<Collider[]>();

        foreach (Transform hitBox in attackPointsHor)
        {
            hitTarget.Add(Physics.OverlapSphere(hitBox.position, attackSizeHorz, targetLayer));
        }

        foreach (Collider[] targetList in hitTarget)
        {
            foreach (Collider c in targetList)
            {
                base.DealDamage(attackDmgSlash, knockbackHor);
                return; //pnly damage player once
            }
        }
    }

    private void AttackVert()
    {
        List<Collider[]> hitTarget = new List<Collider[]>();

        foreach (Transform hitBox in attackPointsVert)
        {
            hitTarget.Add(Physics.OverlapSphere(hitBox.position, attackSizeVert, targetLayer));
        }

        foreach (Collider[] targetList in hitTarget)
        {
            foreach (Collider c in targetList)
            {
                base.DealDamage(attackDmgOverhead, knockbackVert);
                return; //pnly damage player once
            }
        }
    }

    private void moveTowardsTarget()
    {
        // Change animator to walk01
        Vector3 direction = currentTarget.transform.position - transform.position;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
        rb.velocity = new Vector3(horizontalDirection.x * moveSpeed, rb.velocity.y, horizontalDirection.z * moveSpeed);
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        spriteRenderer.color = hurtColor;

        //start the color change so it doesn't start mid combo
        if (colorChangeCoroutine != null)
            StopCoroutine(colorChangeCoroutine);

        colorChangeCoroutine = StartCoroutine(ChangeColor());

        if (canAttack)
        {
            //start the disablemove so it doesn't start mid combo
            if (disableMoveCoroutine != null)
                StopCoroutine(disableMoveCoroutine);

            disableMoveCoroutine = StartCoroutine(DisableMovementForSeconds(0.5f));
        }


        //inflict damage
        base.TakeDamage(damage, knockback, direction);

        //die if dead
        //make sure they're not already dying, prevent from calling "die" twice
        if (curHealthPoints <= 0 && !animator.GetBool("isDead"))
        {
            //Debug.Log(gameObject.name + " Fucking Died");
            canMove = false;
            Die();
            StopCoroutine(colorChangeCoroutine);
            StopCoroutine(disableMoveCoroutine);
        }
    }

    private IEnumerator ChangeColor()
    {
        //this changes the color, duh
        float tick = 0f;
        while (spriteRenderer.color != regularColor)
        {
            tick += Time.deltaTime * colorSpeed;
            spriteRenderer.color = Color.Lerp(hurtColor, regularColor, tick);
            yield return null;
        }
    }

    protected IEnumerator DisableMovementForSeconds(float seconds)
    {
        canMove = false;
        yield return new WaitForSeconds(seconds);
        canMove = true;
    }

    private void ResetAttack()
    {
        //add some randomness to the next time he'll attack
        nextDamageTime = Time.time + damageInterval;
        canAttack = false;
        isAttacking = false;
    }

    protected void PlayAttackVFX(Vector3 direction, GameObject vfxObj)
    {
        vfxObj.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z));
        vfxObj.SetActive(true);
    }

    public virtual void DisableAttackVFX()
    {
        vfxHorizObj.SetActive(false);
        vfxVertObj.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        foreach (Transform t in attackPointsHor)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(t.position, attackSizeHorz);
        }

        foreach (Transform t in attackPointsVert)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position, attackSizeVert);
        }
    }
}
