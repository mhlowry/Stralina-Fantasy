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
    private bool canMove = true;

    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        SlideTowardsPlayer();
    }

    private void SlideTowardsPlayer()
    {
        if (playerObject != null && aggroRange(aggroDistance) && canMove)
        {
            // Change animator to walk01
            Vector3 direction = playerObject.transform.position - transform.position;
            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;
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
