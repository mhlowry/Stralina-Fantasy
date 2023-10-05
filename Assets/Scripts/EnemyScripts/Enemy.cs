using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealthPoints = 10;
    [SerializeField] private int expWorth;
    [SerializeField] protected float knockbackMultiplier = 1f;
    protected int curHealthPoints;
    bool isDead = false;

    //Game components that will be generally needed
    [SerializeField] protected Rigidbody rb;
    protected GameObject playerObject;

    protected virtual void Awake()
    {
        //sets whatever object this is on to be put on the "enemy" layer, so the player can attack it.
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        gameObject.layer = enemyLayer;

        //sets enemy's current health to max health on awake
        curHealthPoints = maxHealthPoints;

        //Gets rigidbody
        rb = GetComponent<Rigidbody>();

        //Find Player
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void TakeDamage(int damage, float knockBack, Vector3 direction)
    {
        curHealthPoints -= damage;

        //calculate vector of position relative from the player's position to the enemy's position
        Vector3 enemyPosition = gameObject.transform.position;
        Vector3 playerPosition = playerObject.transform.position;

        // Calculate the direction vector
        Vector3 relativePos = enemyPosition - playerPosition;
        Vector3 normRelative = relativePos.normalized;
        Vector3 normAttack = direction.normalized;

        //adds unit vector of both the direction of the attack, and position relative to the player.  This is to create a dynamic knockback effect
        Vector3 knockbackDir = normRelative + normAttack;

        //Debug.Log("Knockback: " + knockbackDir.normalized);

        rb.AddForce(knockBack * knockbackMultiplier * knockbackDir.normalized, ForceMode.Impulse);
    }

    //disables their collider and destroys the object after some time has passed
    protected virtual void Die()
    {
        isDead = true;
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(DestroyEnemy());
    }

    public bool GetIsDead() { return isDead; }
    public int GetExpWorth() { return expWorth; }
    IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public bool aggroRange(float range)
    {
        //if the player is within the range, return true
        if (Vector3.Distance(playerObject.transform.position, transform.position) < range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
