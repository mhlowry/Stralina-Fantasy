using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingDummy : TestDummy
{
    [SerializeField] private float attackSize = 3.0f;
    [SerializeField] private LayerMask playerLayer;
    protected override void Awake()
    {
        base.Awake();

        maxHealthPoints = 1000;
        curHealthPoints = maxHealthPoints;
        regularColor = Color.green;
        hurtColor = Color.white;
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //there is most certainly a better way to do this but whatever it's a test dummy for a reason
        Collider hitPlayer = Physics.OverlapSphere(transform.position, attackSize, playerLayer)[0];
        hitPlayer.GetComponent<Player>().HealPlayer(damage * 10);
        Debug.Log("UwU");
        base.TakeDamage(damage, knockback, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackSize);
    }
}
