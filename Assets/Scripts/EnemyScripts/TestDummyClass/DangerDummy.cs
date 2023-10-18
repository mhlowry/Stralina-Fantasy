using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerDummy : TestDummy
{ 
    [SerializeField] private float attackSize = 3.0f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] protected int damageDeal = 1;

    protected override void Awake()
    {
        base.Awake();

        maxHealthPoints = 100;
        curHealthPoints = maxHealthPoints;
        regularColor = Color.blue;
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        //there is most certainly a better way to do this but whatever it's a test dummy for a reason
        Collider[] hitPlayer;
        hitPlayer = Physics.OverlapSphere(transform.position, attackSize, playerLayer);

        foreach (Collider collider in hitPlayer)
            collider.GetComponent<Player>().TakeDamage(damageDeal);

        Debug.Log("COUNTER!!!");
        base.TakeDamage(damage, knockback, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackSize);
    }
}
