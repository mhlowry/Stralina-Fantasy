using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerDummy : TestDummy
{

    [SerializeField] private float attackSize = 3.0f;
    [SerializeField] private LayerMask playerLayer;
    private void Awake()
    {
        maxHealthPoints = 100;
        curHealthPoints = maxHealthPoints;
        regularColor = Color.blue;
    }

    public override void TakeDamage(int damage)
    {
        //there is most certainly a better way to do this but whatever it's a test dummy for a reason
        Collider hitPlayer = Physics.OverlapSphere(transform.position, attackSize, playerLayer)[0];
        hitPlayer.GetComponent<Player>().TakeDamage(damage * 10);
        Debug.Log("COUNTER!!!");
        base.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackSize);
    }
}
