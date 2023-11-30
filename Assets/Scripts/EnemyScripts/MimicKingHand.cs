using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CustomGravity))]
public class MimicKingHand : Enemy
{
    [SerializeField] LayerMask targetLayer;

    [SerializeField] float timeAirborne;
    [SerializeField] float timeActive;
    [SerializeField] float gravityScale;
    float spawnTime;

    [SerializeField, Range(0, 14)] private int attackDmgStomp = 3;
    [SerializeField] private float knockbackStomp = 8f;
    [SerializeField] Transform attackPoint;
    [SerializeField] float attackSize;
    [SerializeField] GameObject vfxObj;

    CustomGravity gravity;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        spawnTime = Time.time;
        gravity = GetComponent<CustomGravity>();
    }

    private void Start()
    {
        gravity.gravityScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > spawnTime + timeAirborne)
        {
            gravity.gravityScale = gravityScale;
            PlayAttackVFX();
        }
        if (Time.time > spawnTime + timeActive)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        animator.SetTrigger("impact");

        CameraShake.instance.ShakeCamera(impulseSource);
        AudioManager.instance.PlayAll(new string[] {"impact_4", "rumble_impact" });
        SlamAttack();
    }

    private void SlamAttack()
    {
        Collider[] hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);

        hitTarget = Physics.OverlapSphere(attackPoint.position, attackSize, targetLayer);
        foreach (Collider collider in hitTarget)
            base.DealDamage(attackDmgStomp, knockbackStomp, collider.gameObject);
    }

    private void PlayAttackVFX()
    {
        vfxObj.SetActive(true);
    }

    public override void TakeDamage(int damage, float knockback, Vector3 direction)
    {
        return;
    }

    void OnDrawGizmosSelected()
    {
        //Draw the hitbox
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackSize);
    }
}
