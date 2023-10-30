using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinDagger : FootSoldier
{
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (playerObject != null && inAggroRange && canMove && !isDead)
        {
            if (inAttackRange && canAttack)
            {

            }
            else
            {
                base.NavigateCombat();
            }
        }
    }
}
