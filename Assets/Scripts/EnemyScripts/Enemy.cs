using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealthPoints = 10;
    protected int curHealthPoints;

    //sets enemy's current health to max health on awake
    void Awake()
    {
        //sets whatever object this is on to be put on the "enemy" layer, so the player can attack it.
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        gameObject.layer = enemyLayer;
        //sets enemy's current health to max health on awake
        curHealthPoints = maxHealthPoints;
    }

    public virtual void TakeDamage(int damage)
    {
        curHealthPoints -= damage;
    }


    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
