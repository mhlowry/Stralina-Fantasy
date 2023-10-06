using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordMan : Player
{
    bool isBlocking = false;
    bool tryingToBlock = false;

    protected override void Update()
    {
        if(tryingToBlock && CanBlock())
        {
            isBlocking = true;
            tryingToBlock = false;
        }

        if(!CanBlock())
        {
            isBlocking = false;
        }

        animGFX.SetBool("isBlocking", isBlocking);

        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        if (isBlocking)
        {
            //animate the blocking
            animGFX.SetTrigger("blockHit");
            //drain the meter
            UseMeter(15f);
        }
        else
        {
            base.TakeDamage(damage);
        }
    }

    public void CallBlocking(InputAction.CallbackContext context)
    {
        //if the player is 
        if (context.canceled)
        {
            isBlocking = false;
            tryingToBlock = false;
            return;
        }
        
        tryingToBlock = true;
    }

    private bool CanBlock()
    {
        return 
            animGFX.GetBool("comboOver")     //is not mid combo
            && !IsStunned()                  //is not stunned
            && !animGFX.GetBool("isRolling") //is not rolling
            && GetCurMeter() >= 15f;         //has enough meter
    }
}
