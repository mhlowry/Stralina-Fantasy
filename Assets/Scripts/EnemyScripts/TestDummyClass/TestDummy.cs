using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummy : Enemy
{
    public float colorSpeed = 0.5f;
    public Color regularColor = Color.white;
    public Color hurtColor = Color.red;

    private Material material;
    private Coroutine colorChangeCoroutine;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        material = GetComponent<Renderer>().material;
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

        //die if dead
        if(curHealthPoints <= 0)
        {
            Debug.Log(gameObject.name + " Fucking Died");
            Die();
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

    protected override void Die()
    {
        base.Die();
        GetComponent<Collider>().enabled = false;
    }
}
