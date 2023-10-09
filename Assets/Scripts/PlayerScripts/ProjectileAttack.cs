using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class ProjectileAttack : PlayerAttack
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float projectileDuration = 10f;

    [SerializeField] private int comboIndex;
    [SerializeField] private GameObject projectilePrefab;

    public int GetIndex() { return comboIndex; }

    public override IEnumerator ActivateAttack(Player player, float dmgMultiplier, float meterGain, LayerMask enemyLayers, UnityEngine.Vector3 direction)
    {
        base.ActivateAttack(player, dmgMultiplier, meterGain, enemyLayers, direction);

        // Create a new instance of the projectile using Instantiate
        GameObject newProjectile = GameObject.Instantiate(projectilePrefab, GetHitBoxes()[0].GetPosition(), UnityEngine.Quaternion.LookRotation(direction));

        // Get the Projectile component from the new projectile if it has one
        ProjectileProperties projectile = newProjectile.GetComponent<ProjectileProperties>();

        // Check if the projectile has a Projectile component
        if (projectile != null)
        {
            // Set the properties of the newly instantiated projectile
            projectile.InitializeProjectile(speed, projectileDuration,
                (int)(GetDamage() * player.GetAttackScale() * dmgMultiplier), GetKnockBack() * player.GetKnockBScale(), direction);
        }

        yield return null;
    }

}