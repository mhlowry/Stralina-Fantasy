using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RepeatingAttack : PlayerAttack
{
    [SerializeField] private int comboIndex;
    [SerializeField] private int numStrikes;
    [SerializeField] private float delayStrikes;

    [SerializeField] private float finalDamage;
    [SerializeField] private float finalKnockBack;
    [SerializeField] private List<HitBox> finalHitBoxes;
    [SerializeField] private GameObject finalVfxObj;

    public int GetIndex() {  return comboIndex; }

    //I beseech you not to cringe at this terrible, terrible spghetti code
    public override IEnumerator ActivateAttack(Player player, float dmgMultiplier, float meterGain, LayerMask enemyLayers, UnityEngine.Vector3 direction)
    {
        yield return new WaitForSeconds(GetDelay());
        DisableAttackVFX();
        PlayAttackVFX(direction);

        //Do the takeDamage scan multiple times
        for (int i = 0; i < numStrikes; ++i)
        {
            if(player.IsStunned())
            {
                yield break;
            }

            List<Collider[]> hitEnemies = new List<Collider[]>();

            foreach (HitBox hitBox in GetHitBoxes())
            {
                hitEnemies.Add(Physics.OverlapSphere(hitBox.GetPosition(), hitBox.GetSize(), enemyLayers));
            }

            //This is to prevent enemies from Getting hit twice if they're in range of 2 or more hitboxes
            HashSet<Collider> loggedEnemies = new HashSet<Collider>();

            foreach (Collider[] enemyList in hitEnemies)
            {
                foreach (Collider enemy in enemyList)
                {
                    if (!loggedEnemies.Contains(enemy))
                    {
                        //Main meter per enemy hit
                        player.GainMeter(meterGain/5);
                        Enemy thisEnemy = enemy.GetComponent<Enemy>();

                        //this is the main attack shit
                        thisEnemy.TakeDamage((int)(GetDamage() * player.GetAttackScale() * dmgMultiplier), GetKnockBack() * player.GetKnockBScale(), direction);
                        if (thisEnemy.GetIsDead())
                            player.GainExp(thisEnemy.GetExpWorth());

                        loggedEnemies.Add(enemy);
                    }
                }
            }

            yield return new WaitForSeconds(delayStrikes);
        }

        //
        if (player.IsStunned())
        {
            yield break;
        }

        //Now we activate the final attack
        DisableAttackVFX();
        FinalPlayAttackVFX(direction);
        List<Collider[]> finalHitEnemies = new List<Collider[]>();

        foreach (HitBox hitBox in finalHitBoxes)
        {
            finalHitEnemies.Add(Physics.OverlapSphere(hitBox.GetPosition(), hitBox.GetSize(), enemyLayers));
        }

        //This is to prevent enemies from Getting hit twice if they're in range of 2 or more hitboxes
        HashSet<Collider> finalLoggedEnemies = new HashSet<Collider>();

        foreach (Collider[] enemyList in finalHitEnemies)
        {
            foreach (Collider enemy in enemyList)
            {
                if (!finalLoggedEnemies.Contains(enemy))
                {
                    //Main meter per enemy hit
                    player.GainMeter(meterGain);
                    Enemy thisEnemy = enemy.GetComponent<Enemy>();

                    //this is the main attack shit
                    thisEnemy.TakeDamage((int)(finalDamage * player.GetAttackScale() * dmgMultiplier), finalKnockBack * player.GetKnockBScale(), direction);
                    if (thisEnemy.GetIsDead())
                        player.GainExp(thisEnemy.GetExpWorth());

                    finalLoggedEnemies.Add(enemy);
                }
            }
        }
    }

    protected void FinalPlayAttackVFX(UnityEngine.Vector3 direction)
    {
        finalVfxObj.transform.rotation = UnityEngine.Quaternion.LookRotation(direction);
        finalVfxObj.SetActive(true);
    }
}
