using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingDummy : TestDummy
{
    [SerializeField] private float attackSize = 3.0f;
    [SerializeField] private int heal = 3;
    [SerializeField] private LayerMask targetLayer;
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
        Collider []hitTarget = Physics.OverlapSphere(transform.position, attackSize, targetLayer);

        foreach (Collider collider in hitTarget)
            collider.GetComponent<Player>().HealPlayer(heal);
        Debug.Log("UwU");
        base.TakeDamage(damage, knockback, direction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackSize);
    }
}
